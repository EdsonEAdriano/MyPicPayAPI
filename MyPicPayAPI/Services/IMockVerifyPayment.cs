namespace SimplePicPay.Integration
{
    public interface IMockVerifyPayment
    {
        Task<string> VerifyPayment();
    }
}
