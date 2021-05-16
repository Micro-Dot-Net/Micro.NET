using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Transport;
using Micro.Net.Extensions;
using Micro.Net.Support;

namespace Micro.Net.Transport.Tcp
{
    public class SecureTcpOptions
	{
		public bool UseSsl { get; init; }
		public X509Certificate2 Certificate { get; init; }
		public IEnumerable<string> AcceptThumbprints { get; init; }
	}
}