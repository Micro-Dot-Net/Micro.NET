using System;
using Micro.Net.Abstractions;

namespace Micro.Net.Dispatch
{
    public class DispatchContextFactory : IContextSubFactory
    {
        public bool TryCreate<TContext>(out TContext context) where TContext : IContextBase
        {
            if (typeof(TContext).IsConstructedGenericType &&
                typeof(TContext).GetGenericTypeDefinition() == typeof(IDispatchContext<,>))
            {
                context = (TContext)typeof(DispatchContext).GetMethod(nameof(DispatchContext.Create))
                    .MakeGenericMethod(typeof(TContext).GetGenericArguments())
                    .Invoke(null, Array.Empty<object>());

                return true;
            }

            context = default;

            return false;
        }
    }
}