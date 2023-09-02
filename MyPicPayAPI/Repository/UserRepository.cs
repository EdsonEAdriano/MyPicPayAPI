using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SimplePicPay.Data;
using SimplePicPay.Helpers;
using SimplePicPay.Integration;
using SimplePicPay.Models;

namespace SimplePicPay.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectionContext _con;
        private readonly IMockVerifyPayment _mockVerifyPayment;
        private readonly ISendEmail _sendEmail;
        private readonly ILogger<UserRepository> _log;   
        public UserRepository(ConnectionContext con, IMockVerifyPayment mockVerifyPayment, ISendEmail sendMail, ILogger<UserRepository> log) 
        {
            _con = con;
            _mockVerifyPayment = mockVerifyPayment;
            _sendEmail = sendMail;
            _log = log;
        }

        public async Task<bool> Add(UserModel user)
        {
            if (await VerifyCPF(user.CPF))
            {
                if (await VerifyEmail(user.Email))
                {
                    await _con.Users.AddAsync(user);
                    await _con.SaveChangesAsync();

                    return true;
                }
                else
                {
                    _log.LogWarning("Este email já foi cadastrado");
                    return false;
                }
                
            }
            else 
            {
                _log.LogWarning("Este CPF já foi cadastrado");
                return false; 
            }           
        }
        
        private void Update(UserModel user)
        {
            _con.Users.Update(user);
            _con.SaveChanges();
        }

        public UserModel Get(int id)
        {
            var user = _con.Users.FirstOrDefault(x => x.Id == id);
            return user;
        }

        public async Task<bool> SendPayment(UserModel payer, UserModel payee, double value)
        {
            try
            {
                string message = await _mockVerifyPayment.VerifyPayment();

                if (message == "Autorizado")
                {
                    payer.Balance -= value;
                    payee.Balance += value;
                    Update(payer);
                    Update(payee);

                    // This email message acept HTML 
                    string payerMsg = "<h1>Pagamento efetuado!</h1>" +
                                      $"<p>Valor de <strong>R${value}</strong> foi enviado para {payee.Name}</p>";

                    string payeeMsg = "<h1>Pagamento recebido!</h1>" +
                                      $"<p>{payer.Name} efetuou uma pagamento no valor de <strong>R${value}</strong> para sua conta.</p>";

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

        private async Task<bool> VerifyCPF(string cpf)
        {
            if (await _con.Users.AnyAsync(x => x.CPF == cpf))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> VerifyEmail(string email)
        {
            if (await _con.Users.AnyAsync(x => x.Email == email))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
