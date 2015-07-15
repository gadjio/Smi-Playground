namespace Smi.Infra.EventStore
{
	using System;
	using Smi.Infra.Messages;

	public interface IDomainRepository
	{
		bool Exist(Guid aggregateRootId);

		TAggregate GetById<TAggregate>(Guid id)
			where TAggregate : class, IEventProvider, new();

		void Save<TAggregate>(TAggregate aggregateRoot)
			where TAggregate : class, IEventProvider;

		void SaveEvent(IDomainEvent domainEvent);

		void ProcessAction<TAggregate, T>(T command, Action<TAggregate, T> action)
			where TAggregate : class, IEventProvider, new()
			where T : BaseDomainCommand;

		void Evict(Guid id);
		
	}
}