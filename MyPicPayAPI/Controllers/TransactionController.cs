using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.ModelsToSend;
using SimplePicPay.Repository.Transaction;
using SimplePicPay.Repository.User;
using SimplePicPay.ViewModels;
using System.Security.Claims;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionController> _log;
        private readonly IMapper _mapper;
        public TransactionController(IUserRepository userRepository, ILogger<TransactionController> log, IMapper mapper, ITransactionRepository transactionRepository)
        {
            _userRepository = userRepository;
            _log = log;
            _mapper = mapper;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendPayment([FromBody] TransactionModelToSend tranModel)
        {
            var payer = _userRepository.Get(tranModel.payerID);
            var payee = _userRepository.Get(tranModel.payeeID);

            try
            {

                if (payer == null || payee == null)
                {
                    _log.LogWarning("Não foi possivel encontrar os usuários.");
                    return NotFound("Não foi possivel encontrar os usuários.");
                }

                var tran = _transactionRepository.Add(payer, payee, tranModel.value);

                if (payer == payee)
                {
                    _transactionRepository.UpdateStatus(tran, TransactionStatus.Error);
                    _log.LogWarning("Pagador é o mesmo que o recebedor.");
                    return BadRequest("Pagador é o mesmo que o recebedor.");
                }

                if (payer.Type == UserType.Store)
                {
                    _transactionRepository.UpdateStatus(tran, TransactionStatus.Error);
                    _log.LogWarning("Lojistas não podem realizar transferências.");
                    return BadRequest("Lojistas não podem realizar transferências.");
                }

                if (tranModel.value <= 0)
                {
                    _transactionRepository.UpdateStatus(tran, TransactionStatus.Error);
                    _log.LogWarning("Valor inválido.");
                    return BadRequest("Valor inválido.");
                }

                if (payer.Balance < tranModel.value)
                {
                    _transactionRepository.UpdateStatus(tran, TransactionStatus.Error);
                    _log.LogWarning("Saldo insuficiente.");
                    return BadRequest("Saldo insuficiente.");
                }

                if (await _transactionRepository.SendPayment(tran))
                {
                    _transactionRepository.UpdateStatus(tran, TransactionStatus.Completed);
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

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            List<TransactionViewModel> transactions;
            var claimUserType = User.FindFirst(ClaimTypes.Role);
            string userType = claimUserType.Value;

            if ( userType != "Admin")
            {
                var claimEmail = User.FindFirst(ClaimTypes.Email);
                string userEmail = claimEmail.Value;

                transactions = _transactionRepository.Get(userEmail);
            }
            else
            {
                transactions = _transactionRepository.Get();
            }

            if (transactions.Count > 0 ) 
            {
                return Ok(transactions);
            }
            else
            {
                return NotFound("Transações não encontradas.");
            }

            
        }

    }
}
