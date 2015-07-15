namespace Smi.Infra.Messages
{
	using System;
	using System.Collections.Generic;

	public interface IDomainEvent : IEvent
	{
		Guid AggregateId { get; set; }
		int Version { get; set; }
		string ByUser { get; set; }
		long Timestamp { get; set; }
        DateTime? DateHappened { get; set; }
		IDictionary<string, object> Parameters { get; set; }

		string GetJSonSerialisation();
	}

	public interface IEvent
	{
		Guid Id { get; }
	}
}