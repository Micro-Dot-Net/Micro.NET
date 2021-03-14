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
}
