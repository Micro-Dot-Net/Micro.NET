using System;
using System.Collections.Generic;
using GreenPipes;

namespace Micro.Net.Transport.FileSystem
{
    public class FileSystemReceiveConfiguration
    {

        public IDictionary<Type, MessageProcessConfiguration> Mappings { get; set; }
    }
}