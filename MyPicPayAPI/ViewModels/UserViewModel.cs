using SimplePicPay.Helpers;

namespace SimplePicPay.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Email { get; set; }
        public double Balance { get; set; }
    }
}
