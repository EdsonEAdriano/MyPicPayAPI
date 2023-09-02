using SimplePicPay.Models;

namespace SimplePicPay.Repository
{
    public interface IUserRepository
    {
        public Task<bool> Add(UserModel user);

        public UserModel Get(int id);

        public Task<bool> SendPayment(UserModel payer, UserModel payee, double value);
    }
}
