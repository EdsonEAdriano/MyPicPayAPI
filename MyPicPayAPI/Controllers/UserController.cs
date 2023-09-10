using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.ModelsToSend;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterUser([FromBody] UserModelToSend userModel)
        {
            var user = new UserModel { Name = userModel.Name, Type = userModel.Type, CPF = userModel.CPF, Email = userModel.Email, Password = EncodePassword(userModel.Password), Balance = userModel.Balance };

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
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] UserModelToSend userModel)
        {
            userModel.Password = EncodePassword(userModel.Password);

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
        [Authorize]
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


        private string EncodePassword(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in base64Encode" + ex.Message);
                return "";
            }
        }

        private string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}