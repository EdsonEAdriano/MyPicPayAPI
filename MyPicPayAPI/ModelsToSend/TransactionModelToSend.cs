namespace SimplePicPay.ModelsToSend
{
    public class TransactionModelToSend
    {
        public int payerID { get; set; }
        public int payeeID { get; set; }
        public double value { get; set; }
    }
}
