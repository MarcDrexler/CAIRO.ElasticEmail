using System;
using System.Collections.Generic;
using System.Threading;

namespace CAIRO.ElasticEmail
{
    public class DefaultRetryStrategy : RetryStrategy
    {
        public DefaultRetryStrategy(TimeSpan retryInterval, int retryCount)
        {
            RetryInterval = retryInterval;
            RetryCount = retryCount;
        }

        public TimeSpan RetryInterval { get; }
        
        public int RetryCount { get; }

        internal T Try<T>(Func<T> action, List<Exception> exceptions)
        {
            for (int retry = 0; retry < RetryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                    {
                        Thread.Sleep(RetryInterval);
                    }
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}