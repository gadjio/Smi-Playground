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

    public class AddPlayerVictoryCommandHandler : BaseCommandHandler<AddPlayerVictoriesCommand>
    {
        private readonly IEntityRepository entityRepository;

        public AddPlayerVictoryCommandHandler(IBus bus, IEventLogService eventLogService, IEntityRepository entityRepository)
            : base(bus, eventLogService)
        {
            this.entityRepository = entityRepository;
        }

        public override IEnumerable<ValidationResult> ValidateCommand(AddPlayerVictoriesCommand command)
        {
            var player = entityRepository.FindFirst<PlayerEntity>(x => x.AggregateRootId == command.PlayerId);

            if (player == null)
            {
                return new List<ValidationResult> { new ValidationResult("This player does not exist") };
            }

            return null;
        }

        public override IDomainEvent EventToPublish(AddPlayerVictoriesCommand command)
        {
            return new PlayerVictoryAdded(command.DehydrateCommand());
        }

        public override Guid GetAggregateRootId(AddPlayerVictoriesCommand command)
        {
            return command.PlayerId;
        }
    }
}