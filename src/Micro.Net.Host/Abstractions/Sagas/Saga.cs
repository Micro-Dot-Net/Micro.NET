using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions.Sagas
{

    public abstract class Saga<TData> where TData : class, ISagaData
    {
        protected internal TData Data { get; internal set; }
    }

}
