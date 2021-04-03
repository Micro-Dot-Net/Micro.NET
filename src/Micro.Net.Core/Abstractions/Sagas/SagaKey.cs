using System;

namespace Micro.Net.Abstractions.Sagas
{
    public readonly struct SagaKey
    {
        public string Value { get; }

        private SagaKey(string value)
        {
            Value = value;
        }

        public static implicit operator string(SagaKey sagaId) => sagaId.Value;

        public static implicit operator SagaKey(string sagaId)
            => new SagaKey(sagaId);

        public static SagaKey NewSagaId()
            => new SagaKey(Guid.NewGuid().ToString());

        public override string ToString()
        {
            return Value;
        }
    }
}