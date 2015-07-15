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

	public class CreatePlayerCommandHandler : BaseCommandHandler<CreatePlayerCommand>
	{
		private readonly IEntityRepository entityRepository;

		public CreatePlayerCommandHandler(IBus bus, IEventLogService eventLogService, IEntityRepository entityRepository) : base(bus, eventLogService)
		{
			this.entityRepository = entityRepository;
		}

		public override IEnumerable<ValidationResult> ValidateCommand(CreatePlayerCommand command)
		{
			var player = entityRepository.FindFirst<PlayerEntity>(x => x.Name == command.PlayerName);

			if (player != null)
			{
				return new List<ValidationResult> {new ValidationResult("This player name is already taken.")};
			}

			return null;
		}

		public override IDomainEvent EventToPublish(CreatePlayerCommand command)
		{
			return new PlayerCreatedEvent(command.DehydrateCommand());
		}

		public override Guid GetAggregateRootId(CreatePlayerCommand command)
		{
			return command.PlayerId;
		}
	}


}