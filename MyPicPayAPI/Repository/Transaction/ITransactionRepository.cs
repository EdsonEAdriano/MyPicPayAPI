using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.ViewModels;

namespace SimplePicPay.Repository.Transaction
{
    public interface ITransactionRepository
    {
        public TransactionModel Add(UserModel payer, UserModel payee, double value);
        public List<TransactionViewModel> Get();
        public List<TransactionViewModel> Get(string email);
        public void UpdateStatus(TransactionModel tran, TransactionStatus status);
        public Task<bool> SendPayment(TransactionModel tran);
    }
}
