using System;
using System.Collections.Generic;
using System.Text;
using Micro.Net.Core.Configuration;

namespace Micro.Net.Abstractions.Hosting
{
    public interface IMicroserviceConfigurable
    {
        void Configure(Action<IMicroConfigurer> configAction);
    }
}
