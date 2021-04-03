namespace Micro.Net.Abstractions
{
    public interface ISerializerCollection
    {
        ISerializer Default { get; }
        ISerializer Get(string name);
    }
}