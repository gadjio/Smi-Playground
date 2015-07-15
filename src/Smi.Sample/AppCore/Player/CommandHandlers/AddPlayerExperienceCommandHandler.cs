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

    public class AddPlayerExperienceCommandHandler : BaseCommandHandler<AddPlayerExperienceCommand>
    {
        private readonly IEntityRepository entityRepository;

        public AddPlayerExperienceCommandHandler(IBus bus, IEventLogService eventLogService, IEntityRepository entityRepository)
            : base(bus, eventLogService)
        {
            this.entityRepository = entityRepository;
        }

        public override IEnumerable<ValidationResult> ValidateCommand(AddPlayerExperienceCommand command)
        {
            var player = entityRepository.FindFirst<PlayerEntity>(x => x.AggregateRootId == command.PlayerId);

            if (player == null)
            {
                return new List<ValidationResult> { new ValidationResult("This player does not exist") };
            }

            return null;
        }

        public override IDomainEvent EventToPublish(AddPlayerExperienceCommand command)
        {
            return new PlayerExperienceAdded(command.DehydrateCommand());
        }

        public override Guid GetAggregateRootId(AddPlayerExperienceCommand command)
        {
            return command.PlayerId;
        }
    }
}