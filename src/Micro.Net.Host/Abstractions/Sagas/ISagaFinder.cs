using System.Threading.Tasks;
using Micro.Net.Host.Abstractions.Storage;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaFinder<TData> where TData : ISagaData
    {
        public interface For<TMessage> where TMessage : ISagaContract
        {
            Task<TData> Find(TMessage message, ISagaFinderContext context);
        }
    }

    public interface IDefaultSagaFinder
    {
        Task<TData> Find<TData, TMessage>(TMessage message, ISagaFinderContext context) where TMessage : ISagaContract;
    }
    public interface IDefaultSagaFinder<TData>
    {
        Task<TData> Find(ISagaContract message, ISagaFinderContext context);
    }
}