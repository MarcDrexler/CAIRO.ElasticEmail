using System;
using System.Collections.Generic;
using System.Threading;

namespace CAIRO.ElasticEmail
{
    public class InfiniteRetryStrategy : RetryStrategy
    {
        public TimeSpan LongestIntervalTime { get; set; }
        public InfiniteRetryStrategy() : this(TimeSpan.FromHours(1)) {}

        public InfiniteRetryStrategy(TimeSpan longestIntervalTime)
        {
            LongestIntervalTime = longestIntervalTime;
        }

        internal T Try<T>(Func<T> action, List<Exception> exceptions)
        {
            double secondsToWait = 0;
            while (true)
            {
                try
                {
                    if (secondsToWait > 0)
                    {
                        secondsToWait = 2*secondsToWait;

                        if (secondsToWait > LongestIntervalTime.TotalSeconds)
                        {
                            secondsToWait = LongestIntervalTime.TotalSeconds;
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(secondsToWait));
                    }
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    if (secondsToWait == 0) secondsToWait = 1;
                }
            }
        }
    }
}