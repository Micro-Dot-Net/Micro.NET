using System;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Messages
{
    public class TransportEnvelope
    {
        private readonly TaskCompletionSource<TransportMessage> _completionSource;

        public TransportEnvelope(TaskCompletionSource<TransportMessage> completionSource)
        {
            _completionSource = completionSource;
        }

        public void Reply(TransportMessage message)
        {
            _completionSource.TrySetResult(message);
        }

        public void Cancel()
        {
            _completionSource.TrySetCanceled();
        }

        public void Error(Exception exception)
        {
            _completionSource.TrySetException(exception);
        }

        public TransportMessage Message { get; set; }
    }
}