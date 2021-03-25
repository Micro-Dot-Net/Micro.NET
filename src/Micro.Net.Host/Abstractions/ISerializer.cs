using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions
{
    public interface ISerializer
    {
        TValue Materialize<TValue>(string value);
        string Serialize<TValue>(TValue value);
    }
}
