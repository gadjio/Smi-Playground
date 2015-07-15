namespace Smi.Infra.EventStore.Aggregate
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Infra.Services;
	using Smi.Infra.Messages;

	public abstract class BaseAggregateRoot : IEventProvider
	{
		private readonly Dictionary<Type, Action<IDomainEvent>> registeredEventsMap;
		private readonly List<IDomainEvent> appliedEvents;

		public Guid Id { get; protected set; }
		public int Version { get; protected set; }
		private int EventVersion { get; set; }

		public abstract void RegisterEvents();

		public BaseAggregateRoot()
		{
			registeredEventsMap = new Dictionary<Type, Action<IDomainEvent>>();
			appliedEvents = new List<IDomainEvent>();
			RegisterEvents();
		}

		protected void RegisterEvent<TEvent>(Action<TEvent> eventHandler) where TEvent : class, IDomainEvent
		{
			registeredEventsMap.Add(typeof(TEvent), theEvent => eventHandler(theEvent as TEvent));
		}

		protected void Apply<TEvent>(TEvent domainEvent) where TEvent : class, IDomainEvent
		{
			domainEvent.AggregateId = Id;
			domainEvent.Version = GetNewEventVersion();
			Apply(domainEvent.GetType(), domainEvent);
			appliedEvents.Add(domainEvent);
		}

		public void LoadFromHistory(IEnumerable<IDomainEvent> domainEvents)
		{			
			foreach (var domainEvent in domainEvents)
			{
				Apply(domainEvent.GetType(), domainEvent);
			}

			Id = domainEvents.First().AggregateId;
			Version = domainEvents.Last().Version;
			EventVersion = Version;

		}

		private void Apply(Type eventType, IDomainEvent domainEvent)
		{
			Action<IDomainEvent> handler;

			if (!registeredEventsMap.TryGetValue(eventType, out handler))
				throw new UnregisteredDomainEventException(string.Format("The requested domain event '{0}' is not registered", eventType.FullName));

			handler(domainEvent);
		}

		public IEnumerable<IDomainEvent> GetChanges()
		{
			return appliedEvents;
		}

		public void UpdateVersion(int newVersion)
		{
			Version = newVersion;
			EventVersion = Version;
		}

		public void Clear()
		{
			appliedEvents.Clear();
		}

		private int GetNewEventVersion()
		{
			return ++EventVersion;
		}

		protected T GetRehydratedParameters<T>(IDictionary<string, object> parameters)
		{
			return DictionaryAdapterMediator.GetAdapter<T>((IDictionary)parameters);
		}
	}
}