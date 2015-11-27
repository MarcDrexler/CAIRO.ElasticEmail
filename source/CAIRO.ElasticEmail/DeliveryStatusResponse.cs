namespace CAIRO.ElasticEmail
{
    public class DeliveryStatusResponse
    {
        public string ErrorMessage { get; set; }
        public ResultType ResultType { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
    }
}