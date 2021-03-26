using Micro.Net.Support;

namespace Micro.Net.Abstractions
{
    public abstract class Feature : Enumeration
    {
        protected Feature(int id, string name) : base(id, name)
        {
        }
    }
}
