using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.Repository;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TransactionController> _log;
        public TransactionController(IUserRepository userRepository, ILogger<TransactionController> log)
        {
            _userRepository = userRepository;
            _log = log;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(string name, UserType type, string cpf, string email, string password, double balance)
        {
            var user = new UserModel { Name = name, Type = type, CPF = cpf, Email = email, Password = password, Balance = balance };

            if (await _userRepository.Add(user))
            {
                _log.LogInformation("Usuário registrado.");
                return Ok("Usuário registrado.");
            }
            else
            {
                _log.LogWarning("Email ou CPF inválidos!");
                return BadRequest("Email ou CPF inválidos!");
            }
        }

        [HttpGet]
        public IActionResult ViewUser(int userId)
        {
            var user = _userRepository.Get(userId);
            return Ok(user);
        }

        [HttpPost("SendPayment")]
        public async Task<IActionResult> SendPayment(int payerID, int payeeID, double value)
        {
            var payer = _userRepository.Get(payerID);
            var payee = _userRepository.Get(payeeID);

            try
            {

                if (payer == null && payee == null)
                {
                    _log.LogWarning("Não foi possivel encontrar os usuários.");
                    return BadRequest("Não foi possivel encontrar os usuários.");
                }

                if (payer == payee)
                {
                    _log.LogWarning("Pagador é o mesmo que o recebedor.");
                    return BadRequest("Pagador é o mesmo que o recebedor.");
                }

                if (payer.Balance < value)
                {
                    _log.LogWarning("Saldo insuficiente.");
                    return BadRequest("Saldo insuficiente.");
                }
            
                if (await _userRepository.SendPayment(payer, payee, value))
                {
                    _log.LogInformation("Transação concluída!");
                    return Ok("Transação concluída!");
                }
                else
                {
                    _log.LogWarning("Falha ao realizar a transferência.");
                    return BadRequest("Falha ao realizar a transferência.");
                }

                
            }
            catch (Exception e)
            {
                _log.LogError($"Ocorreu um erro ao efetuar a transação. ERROR MESAGE: {e.Message}; ");
                return BadRequest($"Ocorreu um erro ao efetuar a transação. ERROR MESAGE: {e.Message}; ");
            }
            

                
     
        }
    }
}
