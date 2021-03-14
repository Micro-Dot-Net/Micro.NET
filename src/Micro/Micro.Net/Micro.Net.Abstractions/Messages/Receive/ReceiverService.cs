using System;
using Micro.Net.Abstractions.Components;

namespace Micro.Net.Abstractions.Messages.Receive
{
    public interface ReceiverService : IComponent<ComponentKind.Transport.Receive>
    {
        event Action<TransportEnvelope> OnReceive;
    }
}