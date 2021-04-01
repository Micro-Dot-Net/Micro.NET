namespace Micro.Net.Abstractions
{
    public interface ISerializerCollection
    {
        public ISerializer Default { get; }
        public ISerializer Get(string name);
    }
}