using System;

namespace Micro.Net
{
    public interface IContract<TResponse>
    {


    }

    public interface IContract : IContract<ValueTuple>
    {

    }
}