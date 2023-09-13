using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimplePicPay.Data;
using SimplePicPay.Helpers;
using SimplePicPay.Integration;
using SimplePicPay.Models;
using SimplePicPay.Repository.User;
using SimplePicPay.ViewModels;

namespace SimplePicPay.Repository.Transaction
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppConnectionDBContext _con;
        private readonly IMockVerifyPayment _mockVerifyPayment;
        private readonly ISendEmail _sendEmail;
        private readonly ILogger<TransactionRepository> _log;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TransactionRepository(AppConnectionDBContext con, IMockVerifyPayment mockVerifyPayment, ISendEmail sendMail, ILogger<TransactionRepository> log, IUserRepository userRepository, IMapper mapper)
        {
            _con = con;
            _mockVerifyPayment = mockVerifyPayment;
            _sendEmail = sendMail;
            _log = log;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<bool> SendPayment(TransactionModel tran)
        {
            var payer = tran.Payer;
            var payee = tran.Payee;

            try
            {
                string message = await _mockVerifyPayment.VerifyPayment();

                if (message == "Autorizado")
                {
                    payer.Balance -= tran.Value;
                    payee.Balance += tran.Value;
                    _userRepository.Update(payer);
                    _userRepository.Update(payee);

                    // This email message acept HTML 
                    string payerMsg = "<h1>Pagamento efetuado!</h1>" +
                                      $"<p>Valor de <strong>R${tran.Value}</strong> foi enviado para {payee.Name}</p>";

                    string payeeMsg = "<h1>Pagamento recebido!</h1>" +
                                      $"<p>{payer.Name} efetuou uma pagamento no valor de <strong>R${tran.Value}</strong> para sua conta.</p>";

                    _sendEmail.SendMail(payer.Email, "Pagamento efetuado", payerMsg);
                    _sendEmail.SendMail(payee.Email, "Pagamento recebido", payeeMsg);


                    _log.LogInformation("Transferência efetuada com sucesso.");
                    return true;
                }
                else
                {
                    _log.LogWarning("Usuário não autorizado.");
                    return false;
                }
            }
            catch (Exception e)
            {               
                _log.LogError($"Ocorreu um erro ao realizar a transferência. ERROR MESSAGE: {e.Message}; ");
                return false;
            }



        }

        public List<TransactionViewModel> Get()
        {
            var transactions = _con.Transactions
                                    .Include(t => t.Payer)
                                    .Include(t => t.Payee)
                                    .AsNoTracking()
                                    .ToList();

            var transactionsView = _mapper.Map<List<TransactionViewModel>>(transactions);
            return transactionsView;
        }

        public List<TransactionViewModel> Get(string email)
        {
            var transactions = _con.Transactions
                                    .Include(t => t.Payer)
                                    .Include(t => t.Payee)
                                    .AsNoTracking()
                                    .Where(t => t.Payer.Email == email || t.Payee.Email == email)
                                    .ToList();

            var transactionsView = _mapper.Map<List<TransactionViewModel>>(transactions);
            return transactionsView;
        }

        public TransactionModel Add(UserModel payerModel, UserModel payeeModel, double valueP)
        {
            var tran = new TransactionModel { PayerID = payerModel.Id, Payer = payerModel, PayeeID = payeeModel.Id, Payee = payeeModel, Status = TransactionStatus.Pending, Value = valueP, StartDate = DateTime.Now };

            try
            {
                _con.Transactions.Add(tran);
                _con.SaveChanges();

                return tran;
            }
            catch (Exception e)
            {
                _log.LogError($"Ocorreu um erro ao adicionar uma nova transação. ERROR MESSAGE: {e.Message}");
                return null;
            }

            
        }       
        public void UpdateStatus(TransactionModel tran, TransactionStatus status)
        {   
            tran.Status = status;
            tran.EndDate = DateTime.Now;

            try
            {
                _con.Transactions.Update(tran);
                _con.SaveChanges();
            }
            catch (Exception e)
            {
                _log.LogError($"Ocorreu um erro ao alterar o status de uma transação. ERROR MESSAGE: {e.Message}");
            }
        }

        
    }
}
