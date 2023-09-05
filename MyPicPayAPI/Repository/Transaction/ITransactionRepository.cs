using SimplePicPay.Helpers;
using SimplePicPay.Models;

namespace SimplePicPay.Repository.Transaction
{
    public interface ITransactionRepository
    {
        public List<TransactionModel> Get();
        public Task<bool> SendPayment(UserModel payer, UserModel payee, double value);
    }
}
