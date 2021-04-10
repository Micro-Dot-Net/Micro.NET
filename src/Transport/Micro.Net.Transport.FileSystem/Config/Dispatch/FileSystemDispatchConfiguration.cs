using System;
using System.Collections.Generic;

namespace Micro.Net.Transport.FileSystem
{
    public class FileSystemDispatchConfiguration
    {
        public IDictionary<Type, MessageProcessConfiguration> Mappings { get; set; }
    }
}