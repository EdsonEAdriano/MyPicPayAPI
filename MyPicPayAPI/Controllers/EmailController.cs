using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Models;

namespace SimplePicPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] LoginModel login)
        {
            CredentialToSendEmail.email = login.email;
            CredentialToSendEmail.password = login.password;


            return Ok("Credenciais cadastradas para envio de Email.");
        }
    }
}
