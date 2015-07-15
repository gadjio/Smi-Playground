namespace Smi.Infra.Impl.Bus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.Core.Logging;
	using Castle.MicroKernel;
	using NHibernate;
	using Smi.Infra.Bus;
	using Smi.Infra.Handlers;
	using Smi.Infra.Messages;

	public class DirectBus : IBus
	{		
		private readonly IKernel microKernel;
		private readonly ILogger logger;		

		public DirectBus(IKernel microKernel, ILogger logger)
		{
			this.microKernel = microKernel;
			this.logger = logger;					
		}		


		public void Send(ICommand command)
		{
			var type = typeof(IHandleCommand<>);
			var genericType = type.MakeGenericType(command.GetType());
			var service = microKernel.Resolve(genericType);
			
			try
			{
				MethodInfo methodInfo = genericType.GetMethod("Execute");
				methodInfo.Invoke(service, new object[] {command});					
			}
			catch (TargetInvocationException ex)
			{
					
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
			
		}


		public void Publish<T>(IEnumerable<T> events) where T : class, IEvent
		{			
			var localEvents = events.ToList();
			if (localEvents.Count() == 1)
			{				
				Publish(localEvents.First());
				return;
			}
			
			PublishToDna(localEvents);
		}

		public void Publish<T>(T @event) where T : class, IEvent
		{
			InvokeFromType(@event.GetType(), @event);

		}



		//private ISession GetSession(Type baseEntityType)
		//{
		//	ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
		//	ISessionScope activeScope = holder.ThreadScopeInfo.GetRegisteredScope();
		//	ISession session = null;
		//	var key = holder.GetSessionFactory(baseEntityType);
		//	if (activeScope == null)
		//	{
		//		session = holder.CreateSession(baseEntityType);
		//	}
		//	else
		//	{
		//		if (activeScope.IsKeyKnown(key))
		//			session = activeScope.GetSession(key);
		//		else
		//			session = holder.GetSessionFactory(baseEntityType).OpenSession();
		//	}

		//	return session;
		//}


		public void PublishToDna<T>(IEnumerable<T> events) where T : class, IEvent
		{			
			foreach (var @event in events)
			{
				InvokeFromType(@event.GetType(), @event);
			}
		}

		private void InvokeFromType<T>(Type t, T @event)
		{
			if (typeof(IEvent).IsAssignableFrom(t))
			{
				
				InvokeHandler(typeof(IHandleEvent<>).MakeGenericType(new[] { t }), @event);	
								
				InvokeFromType(t.BaseType, @event);
			}
		}

		private void InvokeHandler<T>(Type makeGenericType, T @event)
		{
			//logger.Debug("TG: " + makeGenericType.FullName);
			var resolveAll = microKernel.ResolveAll(makeGenericType);
			foreach (var service in resolveAll)
			{				
				//logger.Debug(service.GetType().FullName);
				MethodInfo methodInfo = makeGenericType.GetMethod("Handle");
				methodInfo.Invoke(service, new object[] { @event });
			}
		}

		
	}
}