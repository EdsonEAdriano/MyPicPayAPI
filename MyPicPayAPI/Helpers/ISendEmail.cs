namespace SimplePicPay.Helpers
{
    public interface ISendEmail
    {
        Task<bool> SendMail(string email, string subject, string message);
    }
}
