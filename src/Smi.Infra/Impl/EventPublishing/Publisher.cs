//namespace Dna.Center.Service.Cqrs.Infra.Impl.EventPublishing
//{
//	using System;
//	using Castle.Core.Logging;
//	using Messaging.RabbitMq.Api.Models;
//	using Properties;
//	using Service.Services;
//	using Smi.Infra.EventPublishing;

//	public class Publisher : IPublisher
//	{
//		private readonly IMessagingClientProvider messagingClientProvider;
//		private readonly ILoggingService logger;
//		private readonly bool IsEnable;

//		public Publisher(IMessagingClientProvider messagingClientProvider, ILoggingService logger)
//		{
//			this.messagingClientProvider = messagingClientProvider;
//			this.logger = logger;
//			this.IsEnable = Settings.Default.DnaMessagePublishingEnable;
//			if (IsEnable == false)
//			{
//				logger.LogAppStart("Publisher Ctor - Dna message publishing is not enabled");				
//			}			
//		}


//		public void Publish(string topic, object @event)
//		{
//			if (IsEnable == false)
//			{
//				logger.LogInfoFormat("Dna message publishing is not enabled - Message not sent for topic : {0}", topic);
//				return;
//			}

//			var messagingClient = messagingClientProvider.GetInstance();
//			try
//			{
//				var message = new Message(topic, @event);
//				message.Headers.Add(StandardHeaders.Domain, "Dna");
//				message.Headers.Add(StandardHeaders.From, "Dna.Center");				
//				messagingClient.ProduceOnStandardExchange(message);
//			}
//			catch (Exception)
//			{
//				try
//				{
//					messagingClient.Dispose();
//				}
//				catch
//				{ // We don't want to throw the unable to dispose exception
//				}

//				throw;
//			}

//		}
//	}
	
//}