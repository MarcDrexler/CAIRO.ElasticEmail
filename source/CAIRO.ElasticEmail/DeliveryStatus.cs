using System;

namespace CAIRO.ElasticEmail
{
    public class DeliveryStatus
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public int Recipients { get; set; }
        public int Failed { get; set; }
        public int Delivered { get; set; }
        public int Pending { get; set; }
    }
}