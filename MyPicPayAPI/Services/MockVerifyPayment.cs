using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using System.Net;

namespace SimplePicPay.Integration
{
    public class MockVerifyPayment : IMockVerifyPayment
    {
        private readonly IMockVerifyPaymentRefit _mockVerifyPaymentRefit;
        public MockVerifyPayment(IMockVerifyPaymentRefit mockVerifyPaymentRefit) 
        {
            _mockVerifyPaymentRefit = mockVerifyPaymentRefit;
        }
        public async Task<string> VerifyPayment()
        {
            try
            {
                var message = await _mockVerifyPaymentRefit.VerifyPaymentRefit();
                JObject jsonObject = JObject.Parse(message);

                message = (string)jsonObject["message"];

                return message;
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return ex.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
            
        }
    }
}
