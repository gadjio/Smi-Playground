namespace Smi.Sample.AppCoreEventHandlers.SmiEntities.PlayerEntity
{
	using AppCore.Player.Events;
	using Infra.Entities;
	using Smi.Infra.Handlers;
	using Smi.Sample.Entities;

	public class PlayerCreatedEventHandler :IHandleEvent<PlayerCreatedEvent>
	{
		private readonly IEntityRepository entityRepository;

		public PlayerCreatedEventHandler(IEntityRepository entityRepository)
		{
			this.entityRepository = entityRepository;
		}

		public void Handle(PlayerCreatedEvent @event)
		{
			var parameters = @event.RehydratedParameters;

			var player = new PlayerEntity
			{
				AggregateRootId = parameters.PlayerId,
				Name = parameters.PlayerName
			};

			entityRepository.Save(player);
		}
	}
}