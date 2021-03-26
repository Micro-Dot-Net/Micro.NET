namespace Micro.Net.Abstractions
{
    public interface IResolvable
    {
        bool IsResolved { get; }
        void SetResolve();
    }
}