using SimplePicPay.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplePicPay.Models
{
    public class TransactionModel
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Payer")]
        public int PayerID { get; set; }
        public virtual UserModel Payer { get; set; }
        [ForeignKey("Payee")]
        public int PayeeID { get; set; }
        public virtual UserModel Payee { get; set; }
        public double Value { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
     
    }
}
