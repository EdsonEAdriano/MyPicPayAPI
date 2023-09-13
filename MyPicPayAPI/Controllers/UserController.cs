using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.ModelsToSend;
using SimplePicPay.Repository.User;
using SimplePicPay.ViewModels;
using System.Security.Claims;

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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] UserModelToSend userModel)
        {
            var user = new UserModel { Name = userModel.Name, Type = userModel.Type, CPF = userModel.CPF, Email = userModel.Email, Password = userModel.Password, Balance = userModel.Balance };

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
        public IActionResult Get()
        {
            var user = _userRepository.Get();
            var userView = _mapper.Map<List<UserViewModel>>(user);
            return Ok(userView);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            var user = _userRepository.Get(id);
            
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            else
            {
                var userView = _mapper.Map<UserViewModel>(user);
                return Ok(userView);
            }            
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] UserModelToSend userModel)
        {
            if ( await _userRepository.Update(id, userModel))
            {
                return Ok("Usuário alterado!");
            }
            else
            {
                return BadRequest("Falha ao realizar a alteração.");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (_userRepository.Delete(id))
            {
                return Ok("Usuário excluído!");
            }
            else
            {
                return BadRequest("Usuário não encontrado.");
            }
        }
    }
}