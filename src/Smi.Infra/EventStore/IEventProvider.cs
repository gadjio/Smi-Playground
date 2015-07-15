namespace Smi.Infra.EventStore
{
	using System;
	using System.Collections.Generic;
	using Smi.Infra.Messages;

	public interface IEventProvider
	{
		void Clear();
		void LoadFromHistory(IEnumerable<IDomainEvent> domainEvents);
		Guid Id { get; }
		int Version { get; }
		IEnumerable<IDomainEvent> GetChanges();
		void UpdateVersion(int version);
	}
}