using Microsoft.EntityFrameworkCore;
using SimplePicPay.Data;
using SimplePicPay.Helpers;
using SimplePicPay.Integration;
using SimplePicPay.Models;
using SimplePicPay.Repository.User;

namespace SimplePicPay.Repository.Transaction
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ConnectionContext _con;
        private readonly IMockVerifyPayment _mockVerifyPayment;
        private readonly ISendEmail _sendEmail;
        private readonly ILogger<TransactionRepository> _log;
        private readonly IUserRepository _userRepository;

        public TransactionRepository(ConnectionContext con, IMockVerifyPayment mockVerifyPayment, ISendEmail sendMail, ILogger<TransactionRepository> log, IUserRepository userRepository)
        {
            _con = con;
            _mockVerifyPayment = mockVerifyPayment;
            _sendEmail = sendMail;
            _log = log;
            _userRepository = userRepository;
        }

        public async Task<bool> SendPayment(UserModel payer, UserModel payee, double value)
        {
            var tran = Add(payer, payee, value);

            try
            {
                string message = await _mockVerifyPayment.VerifyPayment();

                if (message == "Autorizado")
                {
                    payer.Balance -= value;
                    payee.Balance += value;
                    _userRepository.Update(payer);
                    _userRepository.Update(payee);

                    // This email message acept HTML 
                    string payerMsg = "<h1>Pagamento efetuado!</h1>" +
                                      $"<p>Valor de <strong>R${value}</strong> foi enviado para {payee.Name}</p>";

                    string payeeMsg = "<h1>Pagamento recebido!</h1>" +
                                      $"<p>{payer.Name} efetuou uma pagamento no valor de <strong>R${value}</strong> para sua conta.</p>";

                    _sendEmail.SendMail(payer.Email, "Pagamento efetuado", payerMsg);
                    _sendEmail.SendMail(payee.Email, "Pagamento recebido", payeeMsg);


                    UpdateStatus(tran, TransactionStatus.Completed);
                    _log.LogInformation("Transferência efetuada com sucesso.");
                    return true;
                }
                else
                {
                    UpdateStatus(tran, TransactionStatus.Unauthorized);
                    _log.LogWarning("Usuário não autorizado.");
                    return false;
                }
            }
            catch (Exception e)
            {
                UpdateStatus(tran, TransactionStatus.Error);
                _log.LogError($"Ocorreu um erro ao realizar a transferência. ERROR MESSAGE: {e.Message}; ");
                return false;
            }



        }

        public List<TransactionModel> Get()
        {
            var transactions = _con.Transactions.ToList();

            return transactions;
        }

        private TransactionModel Add(UserModel payer, UserModel payee, double value)
        {
            var tran = new TransactionModel { Payer = payer, Payee = payee, Status = TransactionStatus.Pending, StartDate = DateTime.Now };

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
        public bool UpdateStatus(TransactionModel tran, TransactionStatus status)
        {   
            tran.Status = status;
            tran.EndDate = DateTime.Now;

            try
            {
                _con.Transactions.Update(tran);
                _con.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                _log.LogError($"Ocorreu um erro ao alterar o status de uma transação. ERROR MESSAGE: {e.Message}");
                return false;
            }
        }

        
    }
}
