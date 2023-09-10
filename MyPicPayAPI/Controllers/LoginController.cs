using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Models;
using SimplePicPay.Services;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Authentication([FromBody] LoginModel login
            )
        {
            CredentialToSendEmail.email = login.email;
            CredentialToSendEmail.password = login.password;


            var token = TokenService.GenerateToken(login.email, login.password);
            return Ok(token);
        }
    }
}
