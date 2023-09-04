using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Services;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Authentication(string username, string password)
        {
            if (username.ToUpper() != "ADMIN" || password != "1")
            {
                return BadRequest("Login inválido!");
            }


            var token = TokenService.GenerateToken(username, password);
            return Ok(token);
        }
    }
}
