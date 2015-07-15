namespace Smi.Sample.AppCore.GameBoard.CommandHandlers
{
	using Commands;
	using Domain;
	using Smi.Infra.EventStore;
	using Smi.Infra.Handlers;

	public class CreateGameBoardCommandHandler : IHandleCommand<CreateGameBoardCommand>
	{
		private readonly IDomainRepository domainRepository;

		public CreateGameBoardCommandHandler(IDomainRepository domainRepository)
		{
			this.domainRepository = domainRepository;
		}

		public void Execute(CreateGameBoardCommand command)
		{
			var gameBoard = new GameBoardAggregateRoot(command.AggregateRootId, command.DehydrateCommand());
			domainRepository.Save(gameBoard);
		}

		public void OnFail(CreateGameBoardCommand command)
		{
			domainRepository.Evict(command.AggregateRootId);
		}
	}
}