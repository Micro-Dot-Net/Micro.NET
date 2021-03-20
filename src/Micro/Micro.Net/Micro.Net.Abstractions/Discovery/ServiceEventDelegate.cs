using System;

namespace Micro.Net.Abstractions.Discovery
{
    public delegate void ServiceEventDelegate(Uri serviceAddress, string contractName, string assemblyName);
}