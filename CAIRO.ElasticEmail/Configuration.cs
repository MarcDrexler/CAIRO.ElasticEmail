using System;

namespace CAIRO.ElasticEmail
{
    public class Configuration
    {
        public Configuration()
        {
            PreferredRetryStrategy = new DefaultRetryStrategy(TimeSpan.FromSeconds(5), 3);
        }

        public RetryStrategy PreferredRetryStrategy { get; set; }
        public string ApiKey { get; set; }
    }
}