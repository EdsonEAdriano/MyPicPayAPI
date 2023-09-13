using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SimplePicPay.Data;
using SimplePicPay.Helpers;
using SimplePicPay.Integration;
using SimplePicPay.Models;
using SimplePicPay.ModelsToSend;

namespace SimplePicPay.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppConnectionDBContext _con;
        private readonly ILogger<UserRepository> _log;
        public UserRepository(AppConnectionDBContext con, ILogger<UserRepository> log)
        {
            _con = con;
            _log = log;
        }

        public async Task<bool> Add(UserModel user)
        {
            if (await VerifyCPF(user.CPF))
            {
                if (await VerifyEmail(user.Email))
                {
                    user.Password = EncodePassword(user.Password);
                    await _con.Users.AddAsync(user);
                    await _con.SaveChangesAsync();

                    return true;
                }
                else
                {
                    _log.LogWarning("Este email já foi cadastrado");
                    return false;
                }

            }
            else
            {
                _log.LogWarning("Este CPF já foi cadastrado");
                return false;
            }
        } 

        public async Task<bool> Update(int id, UserModelToSend userModel)
        {
            var user = _con
                .Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null && !await VerifyCPF(userModel.CPF) && !await VerifyEmail(userModel.Email))
            {
                return false;
            }
            else
            {
                user.Name = userModel.Name;
                user.Type = userModel.Type;
                user.CPF = userModel.CPF;
                user.Email = userModel.Email;
                user.Password = EncodePassword(userModel.Password);
                user.Balance = userModel.Balance;


                _con.Users.Update(user);
                _con.SaveChanges();
                return true;
            }           
        }

        public void Update(UserModel user)
        {
            _con.Users.Update(user);
            _con.SaveChanges();
        }

        public bool Delete(int id)
        {
            var user = _con
                .Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return false;
            }
            else
            {
                _con.Users.Remove(user);
                _con.SaveChanges();
                return true;
            }          
        }

        public UserModel Get(int id)
        {
            var user = _con
                .Users
                .FirstOrDefault(x => x.Id == id);

            return user;
        }

        public List<UserModel> Get()
        {
            var users = _con
                .Users
                .ToList();

            return users;
        }

        private async Task<bool> VerifyCPF(long cpf)
        {
            if (await _con.Users.AnyAsync(x => x.CPF == cpf))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> VerifyEmail(string email)
        {
            if (await _con.Users.AnyAsync(x => x.Email == email.Trim()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public UserModel GetByLogin(LoginModel login)
        {
            var user = _con
                .Users
                .FirstOrDefault
                (
                    x => x.Email == login.email
                );

            if (user != null && DecodeFrom64(user.Password) == login.password)
            {
                return user;
            }

            return null;
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
