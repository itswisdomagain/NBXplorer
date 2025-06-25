using NBitcoin.RPC;
using NBXplorer.Configuration;
using System.Collections.Generic;
using System.Net.Http;

namespace NBXplorer
{
	public class RPCClientProvider
	{
		Dictionary<string, RPCClient> _ChainConfigurations = new Dictionary<string, RPCClient>();
		public RPCClientProvider(ExplorerConfiguration configuration, IHttpClientFactory httpClientFactory)
		{
			foreach(var config in configuration.ChainConfigurations)
			{
				var rpc = config?.RPC;
				if (rpc != null)
				{
					rpc.HttpClient = httpClientFactory.CreateClient(nameof(RPCClientProvider));
					if (config.CryptoCode == "DCR")
					{
						// TODO: Correctly handle self signed certificates.
						var handler = new HttpClientHandler();
						handler.ClientCertificateOptions = ClientCertificateOption.Manual;
						handler.ServerCertificateCustomValidationCallback =
						    (httpRequestMessage, cert, cetChain, policyErrors) =>
						{
						    return true;
						};
						rpc.HttpClient = new HttpClient(handler);
					}
					_ChainConfigurations.Add(config.CryptoCode, rpc);
				}
			}
		}

		public RPCClient Get(NBXplorerNetwork network)
		{
			_ChainConfigurations.TryGetValue(network.CryptoCode, out RPCClient rpc);
			return rpc;
		}
	}
}
