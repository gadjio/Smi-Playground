namespace Smi.Infra.EventStore.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Castle.MicroKernel;
	using Smi.Infra.Messages;

	public interface IEventConvertor<TSourceEvent>
	{
		/// <summary>
		/// If return null the event will be ignored
		/// </summary>
		/// <param name="sourceEvent"></param>
		/// <returns></returns>
		object Convert(TSourceEvent sourceEvent);
		Type GetTargetType { get; }
	}

	public abstract class BaseEventConvertor<TSourceEvent, TTargetEvent> : IEventConvertor<TSourceEvent> 
		where TSourceEvent : IDomainEvent 
		where TTargetEvent : IDomainEvent
	{
		protected abstract TTargetEvent CreateTargetEvent(TSourceEvent sourceEvent);

		protected abstract void AdditionnalHandling(TSourceEvent sourceEvent, TTargetEvent targetEvent);

		public object Convert(TSourceEvent sourceEvent)
		{
			var targetEvent = CreateTargetEvent(sourceEvent);

			CopyBaseEventInformation(sourceEvent, targetEvent);

			AdditionnalHandling(sourceEvent, targetEvent);

			return targetEvent;
		}
		public Type GetTargetType { get { return typeof (TTargetEvent); } }

		protected void CopyBaseEventInformation(IDomainEvent sourceEvent, IDomainEvent targetEvent)
		{
			targetEvent.AggregateId = sourceEvent.AggregateId;
			targetEvent.ByUser = sourceEvent.ByUser;
			targetEvent.DateHappened = sourceEvent.DateHappened;
			targetEvent.Timestamp = sourceEvent.Timestamp;
			targetEvent.Version = sourceEvent.Version;
		}

		protected static IDictionary<string, object> Dehydrate(object obj)
		{
			var result = new Dictionary<string, object>();
			PropertyInfo[] propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var propertyInfo in propertyInfos)
			{
				result.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
			}

			return result;
		}
	}

	public interface IEventConvertor
	{
		IDomainEvent ConvertDomainEvent(Type eventType, IDomainEvent domainEvent);
	}

	public class EventConvertorService : IEventConvertor
	{
		private readonly IKernel kernel;
		private MethodInfo convertMethod;

		public EventConvertorService(IKernel kernel)
		{
			convertMethod = GetType().GetMethod("Convert");
			this.kernel = kernel;
		}

		public IDomainEvent ConvertDomainEvent(Type eventType, IDomainEvent domainEvent)
		{
			var convertedEvent = convertMethod.MakeGenericMethod(eventType).Invoke(this, new[] { domainEvent });
			return (IDomainEvent)convertedEvent;
		}

		public object Convert<T>(T sourceEvent)
		{
			var convertor = kernel.ResolveAll(typeof(IEventConvertor<T>)).Cast<IEventConvertor<T>>().SingleOrDefault();

			if (convertor == null)
			{
				return sourceEvent;
			}

			var targetedEvent = convertor.Convert(sourceEvent);

			if (targetedEvent == null)
			{
				return null;
			}

			return convertMethod.MakeGenericMethod(convertor.GetTargetType).Invoke(this, new[] { targetedEvent });
		}


	}
}