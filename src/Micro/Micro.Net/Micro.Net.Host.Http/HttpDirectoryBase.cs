using System;
using System.Collections.Generic;
using System.Linq;
using Micro.Net.Abstractions;

namespace Micro.Net.Host.Http
{
    public abstract class HttpDirectoryBase : IDirectory<(Type, Type), string>
    {
        private object _lockObj = new object();
        private Dictionary<(Type, Type), string> _forward = new Dictionary<(Type, Type), string>();

        public IReadOnlyDictionary<(Type, Type), string> Forward => _forward;

        public IReadOnlyDictionary<string, (Type, Type)> Reverse =>
            _forward.ToDictionary(x => x.Value, y => y.Key);

        public void Add<TRequest, TResponse>(string path)
        {
            lock (_lockObj)
            {
                if (_forward.ContainsValue(path.ToLower()))
                {
                    throw new InvalidOperationException("Directory cannot hold more than one value for each path!");
                }
                
                _forward.Add((typeof(TRequest), typeof(TResponse)), path.ToLower());
            }
        }
    }
}