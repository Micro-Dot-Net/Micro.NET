using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Core.Abstractions.Management
{
    public interface IHealthProvider : IMicroComponent
    {
        Task<HealthReport> CheckUp();
    }
}
