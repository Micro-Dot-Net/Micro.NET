using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Net.Abstractions;

namespace Micro.Net.Core.Context
{
    public class ContextFactory : IContextFactory
    {
        private readonly IEnumerable<IContextSubFactory> _subFactories;

        public ContextFactory(IEnumerable<IContextSubFactory> subFactories)
        {
            _subFactories = subFactories;
        }

        public bool TryCreate<TContext>(out TContext context) where TContext : IContextBase
        {
            foreach (IContextSubFactory contextSubFactory in _subFactories)
            {
                if (contextSubFactory.TryCreate(out context))
                {
                    return true;
                }
            }

            context = default;

            return false;
        }
    }
}
