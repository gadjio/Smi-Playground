namespace Smi.Sample.AppCore.GameBoard.CommandHandlers
{
	using Commands;
	using Domain;
	using Smi.Infra.CommandHandlers;
	using Smi.Infra.EventStore;

    public class AddPlayerToBoardGameCommandHandler : BaseDomainCommandHandler<AddPlayerToBoardGameCommand, GameBoardAggregateRoot>
	{
		public AddPlayerToBoardGameCommandHandler(IDomainRepository domainRepository) : base(domainRepository)
		{
		}

		public override void Process(GameBoardAggregateRoot aggregate, AddPlayerToBoardGameCommand command)
		{
			aggregate.AddPlayer(command.DehydrateCommand());
		}
	}
}