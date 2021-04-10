namespace Micro.Net.Abstractions
{
    public interface IContextBase : IFaultable, ITerminable, IResolvable
    {
        ContextStatus Status { get; }
    }
}