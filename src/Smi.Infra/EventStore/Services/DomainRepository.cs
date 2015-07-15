namespace Smi.Infra.EventStore.Services
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using Smi.Infra.Bus;
	using Smi.Infra.Messages;
	using ToBeImplemented;

	public class DomainRepository : IDomainRepository
	{
		private readonly IDomainEventStorage domainEventStorage;
		private readonly IBus bus;
		private readonly IEventConvertor eventConvertor;
		private readonly ConcurrentDictionary<Guid, object> cache = new ConcurrentDictionary<Guid, object>();
		private readonly ConcurrentDictionary<Guid, object> locks = new ConcurrentDictionary<Guid, object>();
		private object locker = new object();


		public DomainRepository(IDomainEventStorage domainEventStorage, IBus bus, IEventConvertor eventConvertor)
		{
			this.domainEventStorage = domainEventStorage;
			this.eventConvertor = eventConvertor;
			this.bus = bus;
		}

		public void ProcessAction<TAggregate, T>(T command, Action<TAggregate, T> action)
			where TAggregate : class, IEventProvider, new()
			where T : BaseDomainCommand
		{
			if (!locks.ContainsKey(command.AggregateRootId))
			{
				locks.TryAdd(command.AggregateRootId, new object());
			}
			var lockKey = locks[command.AggregateRootId];

			lock (lockKey)
			{				
				var aggregate = GetById<TAggregate>(command.AggregateRootId);
				action.Invoke(aggregate, command);
				Save(aggregate);
			}
		}

		public bool Exist(Guid id)
		{
			if (cache.ContainsKey(id))
			{
				return true;
			}

			return domainEventStorage.GetAllEvents(id).Any();

		}

		public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IEventProvider, new()
		{
			if (cache.ContainsKey(id))
			{
				return (TAggregate)cache[id];
			}

			lock (locker)
			{
				if (cache.ContainsKey(id))
					return (TAggregate)cache[id];

				var aggregateRoot = new TAggregate();
				LoadRemainingHistoryEvents(id, aggregateRoot);
				cache.TryAdd(id, aggregateRoot);
				return aggregateRoot;
			}
		}

		public void Save<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IEventProvider
		{
			var entity = (IEventProvider)aggregateRoot;

			domainEventStorage.Save(entity);

			cache[aggregateRoot.Id] = aggregateRoot;

			var changes = entity.GetChanges().ToList();
			entity.Clear();

			bus.Publish(changes.AsEnumerable());
		}

		public void SaveEvent(IDomainEvent domainEvent)
		{
			domainEventStorage.Save(domainEvent);
			bus.Publish(domainEvent);
		}

		private void LoadRemainingHistoryEvents(Guid id, IEventProvider aggregateRoot)
		{
			var events = domainEventStorage.GetAllEvents(id, aggregateRoot.Version);

			var convertedEvents = new List<IDomainEvent>();
			foreach (var domainEvent in events)
			{
				var @event = eventConvertor.ConvertDomainEvent(domainEvent.GetType(), domainEvent);
				if (@event != null)
				{
					convertedEvents.Add(@event);
				}
			}

			aggregateRoot.LoadFromHistory(convertedEvents);
		}

		public void Evict(Guid id)
		{
			object removed;
			cache.TryRemove(id, out removed);
		}
	}
}