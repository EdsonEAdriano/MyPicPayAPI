using SimplePicPay.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SimplePicPay.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public UserType Type { get; set; }
        public long CPF { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public double Balance { get; set; }
    }
}