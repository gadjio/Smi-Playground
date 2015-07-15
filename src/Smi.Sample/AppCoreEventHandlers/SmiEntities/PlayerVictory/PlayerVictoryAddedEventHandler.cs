namespace Smi.Sample.AppCoreEventHandlers.SmiEntities.PlayerVictory
{
    using AppCore.Player.Events;
    using Infra.Entities;
    using Smi.Infra.Handlers;
    using Entities;

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

            var playerVictory = entityRepository.FindFirst<PlayerVictory>(x => x.AggregateRootId == parameters.PlayerId);

            if (playerVictory == null)
            {
                playerVictory = new PlayerVictory
                {
                    AggregateRootId = parameters.PlayerId,
                    NumberOfVictory = parameters.VictoriesToAdd
                };
                entityRepository.Save(playerVictory);
            }
            else
            {
                playerVictory.NumberOfVictory += parameters.VictoriesToAdd;
                entityRepository.Update(playerVictory);
            }

        }
    }
}