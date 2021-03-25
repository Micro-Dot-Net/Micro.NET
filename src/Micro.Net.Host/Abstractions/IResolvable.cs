namespace Micro.Net
{
    public interface IResolvable
    {
        bool IsResolved { get; }
        void SetResolve();
    }
}