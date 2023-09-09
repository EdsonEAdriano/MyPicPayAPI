using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SimplePicPay.Data;
using SimplePicPay.Helpers;
using SimplePicPay.Integration;
using SimplePicPay.Models;

namespace SimplePicPay.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectionContext _con;
        private readonly ILogger<UserRepository> _log;
        public UserRepository(ConnectionContext con, ILogger<UserRepository> log)
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

        public void Update(UserModel user)
        {
            _con.Users.Update(user);
            _con.SaveChanges();
        }

        public UserModel Get(int id)
        {
            var user = _con.Users.FirstOrDefault(x => x.Id == id);
            return user;
        }

        private async Task<bool> VerifyCPF(string cpf)
        {
            if (await _con.Users.AnyAsync(x => x.CPF == cpf.Trim()))
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
