namespace Micro.Net.Abstractions
{
    public interface ISerializerCollection
    {
        ISerializer Default { get; }
        ISerializer Get(string name);
        bool TryGet(string name, out ISerializer serializer);
    }
}