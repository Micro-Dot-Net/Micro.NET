using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Activities
{
    public interface IActivity<TMessage,TLog> where TMessage : IActivityContract<TLog> where TLog : IActivityLog
    {
        Task<TLog> Do(TMessage message, IDoContext context);
        Task Undo(TLog log, IUndoContext context);
    }
}
