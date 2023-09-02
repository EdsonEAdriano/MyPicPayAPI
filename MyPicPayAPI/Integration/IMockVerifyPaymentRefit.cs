using Refit;

namespace SimplePicPay.Integration
{
    public interface IMockVerifyPaymentRefit
    {
        [Get("/v3/8fafdd68-a090-496f-8c9a-3442cf30dae6")]
        Task<string> VerifyPaymentRefit();
    }
} 
