using System;

namespace Micro.Net.Abstractions
{
    public interface IContextFactory
    {
        bool TryCreate<TContext>(out TContext context) where TContext : IContextBase;
    }

    public interface IContextSubFactory
    {
        bool TryCreate<TContext>(out TContext context) where TContext : IContextBase;
    }
}