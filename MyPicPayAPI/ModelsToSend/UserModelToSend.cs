using SimplePicPay.Helpers;

namespace SimplePicPay.ModelsToSend
{
    public class UserModelToSend
    {
        public string Name { get; set; }
        public UserType Type { get; set; }
        public long CPF { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }
    }
}
