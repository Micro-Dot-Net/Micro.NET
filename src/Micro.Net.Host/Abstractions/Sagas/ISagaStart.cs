namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaStartHandler<TMessage> : ISagaStepHandler<TMessage> where TMessage : ISagaContract { }
}