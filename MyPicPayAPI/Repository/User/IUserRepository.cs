using SimplePicPay.Models;
using SimplePicPay.ModelsToSend;

namespace SimplePicPay.Repository.User
{
    public interface IUserRepository
    {
        public Task<bool> Add(UserModel user);
        public void Update(UserModel user); 
        public Task<bool> Update(int id, UserModelToSend user);
        public bool Delete(int id);
        public UserModel Get(int id);
        public List<UserModel> Get();
        public UserModel GetByLogin(LoginModel login);
    }
}
