using System;
using System.Collections.Generic;
using System.Threading;

namespace CAIRO.ElasticEmail
{
    public class RetryLogic
    {
        private readonly RetryStrategy _strategy;

        public RetryLogic(RetryStrategy strategy)
        {
            _strategy = strategy;
        }

        public void Execute(Action action)
        {
            Execute<object>(() =>
            {
                action();
                return null;
            });
        }

        public T Execute<T>(Func<T> action)
        {
            var exceptions = new List<Exception>();

            if (_strategy is InfiniteRetryStrategy)
            {
                var strategy = (InfiniteRetryStrategy) _strategy;
                return strategy.Try(action, exceptions);
            }
            else if (_strategy is DefaultRetryStrategy)
            {
                var strategy = (DefaultRetryStrategy) _strategy;
                return strategy.Try(action, exceptions);
            }

            throw new AggregateException(exceptions);
        }
    }
}