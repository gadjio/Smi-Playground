using Smi.Infra.Handlers;
using Smi.Sample.AppCore.Player.Events;
using Smi.Sample.Infra.Entities;

namespace Smi.Sample.AppCoreEventHandlers.SmiEntities.PlayerExperience
{
    public class PlayerVictoryAddedEventHandler : IHandleEvent<PlayerVictoryAdded>
    {
        private readonly IEntityRepository entityRepository;

        public PlayerVictoryAddedEventHandler(IEntityRepository entityRepository)
        {
            this.entityRepository = entityRepository;
        }

        public void Handle(PlayerVictoryAdded @event)
        {
            var parameters = @event.RehydratedParameters;

            var playerExperience = entityRepository.FindFirst<Entities.PlayerExperience>(x => x.AggregateRootId == parameters.PlayerId);

            if (playerExperience == null)
            {
                playerExperience = new Entities.PlayerExperience
                {
                    AggregateRootId = parameters.PlayerId,
                    Experience = parameters.ExperienceToAdd
                };
                entityRepository.Save(playerExperience);
            }
            else
            {
                playerExperience.Experience += parameters.ExperienceToAdd;
                entityRepository.Update(playerExperience);
            }
        }
    }
}