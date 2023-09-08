using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Services;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Authentication(string email, string password)
        {
            CredentialToSendEmail.email = email;
            CredentialToSendEmail.password = password;


            var token = TokenService.GenerateToken(email, password);
            return Ok(token);
        }
    }
}
