namespace Smi.Infra.ToBeImplemented
{
	using System;
	using System.Collections.Generic;
	using EventStore;
	using Messages;

	public interface IDomainEventStorage
	{
		IEnumerable<IDomainEvent> GetAllEvents(Guid eventProviderId);
		void Save(IEventProvider eventProvider);
		IEnumerable<IDomainEvent> GetAllEvents(Guid eventProviderId, int version);		
		void Save(IDomainEvent domainEvent);
	}
}