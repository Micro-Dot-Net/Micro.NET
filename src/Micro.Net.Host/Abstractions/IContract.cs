using System;

namespace Micro.Net.Abstractions
{
    public interface IContract<TResponse>
    {


    }

    public interface IContract : IContract<ValueTuple>
    {

    }
}