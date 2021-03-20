using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions
{
    public interface IFactory<out TImpl>
    {
        TImpl Create();
    }

    public interface IFactory<out TImpl, in TOpts>
    {
        TImpl Create(TOpts options);
    }

    public interface IDirectory<T1, T2>
    {
        public IReadOnlyDictionary<T1, T2> Forward { get; }
        public IReadOnlyDictionary<T2, T1> Reverse { get; }
    }
}
