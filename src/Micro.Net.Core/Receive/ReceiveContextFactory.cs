using System;
using Micro.Net.Abstractions;
using Micro.Net.Receive;

namespace Micro.Net.Dispatch
{
    public class ReceiveContextFactory : IContextSubFactory
    {
        public bool TryCreate<TContext>(out TContext context) where TContext : IContextBase
        {
            if (typeof(TContext).IsConstructedGenericType &&
                typeof(TContext).GetGenericTypeDefinition() == typeof(IReceiveContext<,>))
            {
                context = (TContext)typeof(ReceiveContext).GetMethod(nameof(ReceiveContext.Create))
                    .MakeGenericMethod(typeof(TContext).GetGenericArguments())
                    .Invoke(null, Array.Empty<object>());

                return true;
            }

            context = default;

            return false;
        }
    }
}