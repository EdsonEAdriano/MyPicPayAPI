using SimplePicPay.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SimplePicPay.Models
{
    public class TransactionModel
    {
        [Key]
        public int Id { get; set; }
        public UserModel Payer { get; set; }
        public UserModel Payee { get; set; }
        public double Value { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
