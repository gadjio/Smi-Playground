namespace Smi.Sample.Infra
{
	using Entities;
	using Model;
	using Smi.Infra.Messages;
	using Smi.Infra.ToBeImplemented;

	public class EventLogService : IEventLogService
	{
		private readonly IEntityRepository entityRepository;

		public EventLogService(IEntityRepository entityRepository)
		{
			this.entityRepository = entityRepository;
		}

		public void SaveEvent(IDomainEvent @event)
		{
			var eventLogEntry = new EventLog
			{
				EventId = @event.Id,
				EventProviderId = @event.AggregateId,
				Version = @event.Version,
				Type = @event.GetType().AssemblyQualifiedName,
				User = @event.ByUser,
				DateHappened = @event.DateHappened,
				JSonDomainEvent = @event.GetJSonSerialisation(),
			};

			entityRepository.Save(eventLogEntry);
		}
	}
}