using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Models;
using SimplePicPay.Repository.User;
using SimplePicPay.Services;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private IUserRepository _userRepository;
        public LoginController(IUserRepository userRepository) 
        { 
            _userRepository = userRepository;
        }

        [HttpPost]
        public IActionResult Authentication([FromBody] LoginModel login)
        {
            var user = _userRepository.GetByLogin(login);

            if (user == null)
            {
                return BadRequest("Login inválido.");
            }
            else
            {
                var token = TokenService.GenerateToken(user);
                return Ok(token);
            }            
        }
    }
}
