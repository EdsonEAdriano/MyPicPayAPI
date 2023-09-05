using SimplePicPay.Models;

namespace SimplePicPay.Repository.User
{
    public interface IUserRepository
    {
        public Task<bool> Add(UserModel user);
        public void Update(UserModel user);

        public UserModel Get(int id);
    }
}
