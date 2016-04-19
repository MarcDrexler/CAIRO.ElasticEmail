using System;

namespace CAIRO.ElasticEmail
{
    public class SendResult
    {
        public ResultType ResultType { get; set; }
        public string ErrorMessage { get; set; }
        public Guid? TransactionId { get; set; }
    }
}