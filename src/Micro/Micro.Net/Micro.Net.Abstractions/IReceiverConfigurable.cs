using System;

namespace Micro.Net.Abstractions
{
    public interface IReceiverConfigurable
    {
        IReceiverConfigurable Configure<TReceiver>(Action<TReceiver> config) where TReceiver : IReceiverConfiguration;
    }
}