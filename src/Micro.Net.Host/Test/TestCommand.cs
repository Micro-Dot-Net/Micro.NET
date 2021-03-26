using System;
using Micro.Net.Abstractions;

namespace Micro.Net.Test
{
    public class TestCommand : IContract
    {
        public Guid Id { get; set; }
    }
}