namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaStartHandler<TMessage> : ISagaStepHandler<TMessage> where TMessage : ISagaContract { }
}