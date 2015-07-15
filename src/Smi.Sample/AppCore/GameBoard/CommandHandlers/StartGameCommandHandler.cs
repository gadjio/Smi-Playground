namespace Smi.Sample.AppCore.GameBoard.CommandHandlers
{
	using Commands;
	using Domain;
	using Smi.Infra.CommandHandlers;
	using Smi.Infra.EventStore;

    public class StartGameCommandHandler : BaseDomainCommandHandler<StartGameCommand, GameBoardAggregateRoot>
	{
		public StartGameCommandHandler(IDomainRepository domainRepository) : base(domainRepository)
		{
		}

		public override void Process(GameBoardAggregateRoot aggregate, StartGameCommand command)
		{
            aggregate.StartGame(command.DehydrateCommand());
		}
	}
}