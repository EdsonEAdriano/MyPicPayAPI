using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.Repository.Transaction;
using SimplePicPay.Repository.User;
using SimplePicPay.ViewModels;

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

        [HttpPost("SendPayment")]
        [Authorize]
        public async Task<IActionResult> SendPayment(int payerID, int payeeID, double value)
        {
            var payer = _userRepository.Get(payerID);
            var payee = _userRepository.Get(payeeID);

            try
            {

                if (payer == null || payee == null)
                {
                    _log.LogWarning("Não foi possivel encontrar os usuários.");
                    return NotFound("Não foi possivel encontrar os usuários.");
                }

                var tran = _transactionRepository.Add(payer, payee, value);

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

                if (payer.Balance < value)
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
                    _transactionRepository.UpdateStatus(tran, TransactionStatus.Error);
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

        [HttpGet("Get")]
        [Authorize]
        public IActionResult Get()
        {
            var transactions = _transactionRepository.Get();
            return Ok(transactions);
        }
    }
}
