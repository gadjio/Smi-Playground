namespace Smi.Sample.AppCore.Player.CommandHandlers
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Commands;
	using Entities;
	using Events;
	using Infra.Entities;
	using Smi.Infra.Bus;
	using Smi.Infra.CommandHandlers;
	using Smi.Infra.Messages;
	using Smi.Infra.ToBeImplemented;

	public class SetPlayerInitialScoreCommandHandler : BaseCommandHandler<SetPlayerInitialScoreCommand>
	{
		private readonly IEntityRepository entityRepository;

		public SetPlayerInitialScoreCommandHandler(IBus bus, IEventLogService eventLogService, IEntityRepository entityRepository) : base(bus, eventLogService)
		{
			this.entityRepository = entityRepository;
		}

		public override IEnumerable<ValidationResult> ValidateCommand(SetPlayerInitialScoreCommand command)
		{
			var playerId = entityRepository.FindFirst<PlayerEntity>(x => x.AggregateRootId == command.PlayerId);

			if (playerId == null)
			{
				return new List<ValidationResult> {new ValidationResult("This player doesn't exist.")};
			}

			return null;
		}

		public override IDomainEvent EventToPublish(SetPlayerInitialScoreCommand command)
		{
			return new PlayerInitialScoreSetEvent(command.DehydrateCommand());
		}

		public override Guid GetAggregateRootId(SetPlayerInitialScoreCommand command)
		{
			return command.PlayerId;
		}
	}
}