namespace Smi.Sample.AppCoreEventHandlers.SmiEntities.PlayerExperience
{
    using AppCore.Player.Events;
    using Infra.Entities;
    using Smi.Infra.Handlers;
    using Entities;

    public class PlayerExperienceAddedEventHandler : IHandleEvent<PlayerExperienceAdded>
    {
        private readonly IEntityRepository entityRepository;

        public PlayerExperienceAddedEventHandler(IEntityRepository entityRepository)
        {
            this.entityRepository = entityRepository;
        }

        public void Handle(PlayerExperienceAdded @event)
        {
            var parameters = @event.RehydratedParameters;

            var playerExperience = entityRepository.FindFirst<PlayerExperience>(x => x.AggregateRootId == parameters.PlayerId);

            if (playerExperience == null)
            {
                playerExperience = new PlayerExperience
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