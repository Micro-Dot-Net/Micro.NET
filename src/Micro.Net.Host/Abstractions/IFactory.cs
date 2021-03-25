using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions
{
    public interface IFactory<out TProduct>
    {
        TProduct Create();
    }

    public interface IFactory<out TProduct, in TOptions>
    {
        TProduct Create(TOptions opts);
    }
}
