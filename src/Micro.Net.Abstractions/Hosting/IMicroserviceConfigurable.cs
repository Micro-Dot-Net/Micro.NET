using System;
using System.Collections.Generic;
using System.Text;
using Micro.Net.Core.Configuration;
using Microsoft.Extensions.Configuration;

namespace Micro.Net.Abstractions.Hosting
{
    public interface IMicroserviceConfigurable
    {
        void Configure(IMicroConfigurer configAction, IConfiguration configuration);
    }
}
