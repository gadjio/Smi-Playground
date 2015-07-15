//using Messaging.RabbitMq.Api.Connections;

//namespace Dna.Center.Service.Cqrs.Infra.Impl.EventPublishing
//{
//	using System;
//	using System.Collections.Generic;
//	using Messaging.RabbitMq.Api;

//	public interface IMessagingClientProvider
//	{
//		IMessagingClient GetInstance();
//	}

//	public class MessagingClientProvider : IMessagingClientProvider
//	{
//		private IMessagingClient instance;
//		private readonly object TheLock = new Object();
//		private IMessagingClientFactory factory;
//		private readonly ClientConfig clientConfig;

//		public MessagingClientProvider(string hostname, string username, string password, ushort heartbeatFrequency)
//		{
//			factory = new MessagingClientFactory();
//			var connectionConfig = new ConnectionConfig
//			{
//				Hostname = hostname,
//				Username = username,
//				Password = password,
//				HeartbeatFrequency = heartbeatFrequency
//			};
//			clientConfig = new ClientConfig
//			{
//				ConnectionConfig = connectionConfig,
//				NamespacesToScan = new List<String>()
//			};
//		}

//		public IMessagingClient GetInstance()
//		{			
//			lock (TheLock) // thread safety
//			{
//				if (instance == null)
//				{
//					instance = factory.CreateClient(clientConfig);
//				}
//			}

//			return instance;			
//		}

//	}
//}