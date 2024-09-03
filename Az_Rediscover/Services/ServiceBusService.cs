using Azure.Messaging.ServiceBus;
using Azure.Identity;
using Azure.Core;
using System.Text;
using System.Text.Json;

namespace Az_Rediscover.Services
{
	public class ServiceBusService
	{
		private readonly ServiceBusClient _serviceBusClient;
		private readonly ServiceBusSender _serviceBusSender;

		private readonly ServiceBusClientOptions clientOptions = new ServiceBusClientOptions
		{
			TransportType = ServiceBusTransportType.AmqpWebSockets
		};

		public ServiceBusService()
		{
			_serviceBusClient = new ServiceBusClient(
				Environment.GetEnvironmentVariable("APPSETTING_ServiceBusNamespace"),
				new DefaultAzureCredential(),
				clientOptions);
			_serviceBusSender = _serviceBusClient.CreateSender(Environment.GetEnvironmentVariable("APPSETTING_ServiceBusQueue"));
		}

		public async Task SendMessageAsync(string message, string title)
		{
			var serviceBusMessage = new ServiceBusMessage(message)
			{
				Subject = title,
			};

			await _serviceBusSender.SendMessageAsync(serviceBusMessage);
		}
	}
}