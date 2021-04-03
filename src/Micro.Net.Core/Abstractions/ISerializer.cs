namespace Micro.Net.Abstractions
{
    public interface ISerializer
    {
        TValue Materialize<TValue>(string value);
        string Serialize<TValue>(TValue value);
    }
}
