using System;
using System.Collections.Generic;

namespace Micro.Net.Storage.Sql
{
    public class SqlStorageConfiguration
    {
        public string ConnectionString { get; set; }
        public IDictionary<string, Type> TableMaps { get; set; }
    }
}