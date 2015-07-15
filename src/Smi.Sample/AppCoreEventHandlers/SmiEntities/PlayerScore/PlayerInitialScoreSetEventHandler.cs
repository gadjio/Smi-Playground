namespace Smi.Sample.AppCoreEventHandlers.SmiEntities.PlayerScore
{
	using AppCore.Player.Events;
	using Entities;
	using Infra.Entities;
	using Smi.Infra.Handlers;

	public class PlayerInitialScoreSetEventHandler :IHandleEvent<PlayerInitialScoreSetEvent>
	{
		private readonly IEntityRepository entityRepository;

		public PlayerInitialScoreSetEventHandler(IEntityRepository entityRepository)
		{
			this.entityRepository = entityRepository;
		}

		public void Handle(PlayerInitialScoreSetEvent @event)
		{
			var parameters = @event.RehydratedParameters;

			var player = new PlayerScore()
			{
				AggregateRootId = parameters.PlayerId,
				Score = parameters.InitialScore
			};

			entityRepository.Save(player);
		}
	}
}