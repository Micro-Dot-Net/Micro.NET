using System;
using Micro.Net.Abstractions.Configuration;

namespace Micro.Net.Configuration
{
    public class ReceiverConfigurable : IReceiverConfigurable
    {
        public IReceiverConfigurable Configure<TReceiver>(Action<TReceiver> config) where TReceiver : IReceiverConfiguration
        {
            throw new NotImplementedException();
        }
    }
}