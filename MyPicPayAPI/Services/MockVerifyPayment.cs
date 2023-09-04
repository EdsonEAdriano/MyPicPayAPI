using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var message = await _mockVerifyPaymentRefit.VerifyPaymentRefit();
            JObject jsonObject = JObject.Parse(message);

            message = (string)jsonObject["message"];

            return message;
        }
    }
}
