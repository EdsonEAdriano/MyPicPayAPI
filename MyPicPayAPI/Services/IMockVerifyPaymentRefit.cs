using Refit;

namespace SimplePicPay.Integration
{
    public interface IMockVerifyPaymentRefit
    {
        [Get("/Authorize")]
        Task<string> VerifyPaymentRefit();
    }
} 
