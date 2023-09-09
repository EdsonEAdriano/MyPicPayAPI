using SimplePicPay.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SimplePicPay.Helpers;

namespace SimplePicPay.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string PayerName { get; set; }
        public string PayeeName { get; set; }
        public double Value { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
