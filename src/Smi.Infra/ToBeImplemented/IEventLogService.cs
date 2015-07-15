namespace Smi.Infra.ToBeImplemented
{
	using Messages;

	public interface IEventLogService
	{
		void SaveEvent(IDomainEvent @event);
	}

	//private void SaveEvent(IDomainEvent @event)
	//{
	//	var eventLogEntry = new EventLog
	//	{
	//		EventId = @event.Id,
	//		EventProviderId = @event.AggregateId,			
	//		Version = @event.Version,
	//		Type = @event.GetType().AssemblyQualifiedName,
	//		User = @event.ByUser,				
	//		DateHappened = @event.DateHappened,
	//		JSonDomainEvent = @event.GetJSonSerialisation(),
	//	};

	//	entityRepository.Save(eventLogEntry);
	//}
}