using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.Repository.User;
using SimplePicPay.ViewModels;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TransactionController> _log;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, ILogger<TransactionController> log, IMapper mapper)
        {
            _userRepository = userRepository;
            _log = log;
            _mapper = mapper;
        }

        [HttpPost("RegisterUser")]
        [Authorize]
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
        [Authorize]
        public IActionResult ViewUser(int userId)
        {
            var user = _userRepository.Get(userId);
            var userView = _mapper.Map<UserViewModel>(user);
            return Ok(userView);
        }
    }
}