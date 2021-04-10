namespace Micro.Net.Abstractions.Sagas
{

    public abstract class Saga<TData> where TData : class, ISagaData
    {
        protected internal TData Data { get; internal set; }
    }
}
