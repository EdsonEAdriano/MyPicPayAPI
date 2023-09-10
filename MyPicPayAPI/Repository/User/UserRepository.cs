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
                user.Password = userModel.Password;
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

    }
}
