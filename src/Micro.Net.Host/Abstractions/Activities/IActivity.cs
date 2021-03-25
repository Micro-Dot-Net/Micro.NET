using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions.Activities
{
    public interface IActivity<TMessage,TLog> where TMessage : IActivityContract<TLog> where TLog : IActivityLog
    {
        Task<TLog> Do(TMessage message, DoContext context);
        Task Undo(TLog log, UndoContext context);
    }
}
