namespace Micro.Net.Abstractions
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
