using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions
{
    public interface IMicroserviceConfigurer
    {
        void ConfigureMicroservice(IMicroserviceConfigurable config);
    }
}
