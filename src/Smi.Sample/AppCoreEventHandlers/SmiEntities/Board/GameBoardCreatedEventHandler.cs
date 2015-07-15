namespace Smi.Sample.AppCoreEventHandlers.SmiEntities.Board
{
	using AppCore.GameBoard.Events;
	using Entities;
	using Infra.Entities;
	using Smi.Infra.Handlers;

	public class GameBoardCreatedEventHandler : IHandleEvent<GameBoardCreatedEvent>
	{
		private readonly IEntityRepository entityRepository;

		public GameBoardCreatedEventHandler(IEntityRepository entityRepository)
		{
			this.entityRepository = entityRepository;
		}

		public void Handle(GameBoardCreatedEvent @event)
		{
			var parameters = @event.RehydratedParameters;

			var board = new BoardEntity {AggregateRootId = @event.AggregateId, };
			
			entityRepository.Save(board);
		}
	}
}