namespace Smi.Sample.AppCore.GameBoard.Domain
{
	using System;
	using System.Collections.Generic;
	using Commands;
	using Events;
	using Smi.Infra.EventStore.Aggregate;

	public class GameBoardAggregateRoot : BaseAggregateRoot
	{
		private int ActualNumberOfPlayers { get; set; }

		private int MinimumNumberOfPlayers { get; set; }
		private int MaximumNumberOfPlayers { get; set; }
        private Boolean IsGameStarted { get; set; }
		private DateTime? StartDate { get; set; }
	    private IList<string> AddedPlayersToBoardGame { get; set; }

	    public override void RegisterEvents()
		{
			RegisterEvent<GameBoardCreatedEvent>(OnGameBoardCreated);
			RegisterEvent<PlayerAddedEvent>(OnPlayerAdded);
            RegisterEvent<GameStartedEvent>(OnGameStarted);
		}

		public GameBoardAggregateRoot()
		{
			this.AddedPlayersToBoardGame = new List<string>();
		}

		public GameBoardAggregateRoot(Guid aggregateRootId, IDictionary<string, object> parameters)
		{
			Id = aggregateRootId;
			Apply(new GameBoardCreatedEvent(parameters));
		    this.AddedPlayersToBoardGame = new List<string>();
		}

		private void OnGameBoardCreated(GameBoardCreatedEvent @event)
		{
			var parameters = @event.RehydratedParameters;

			MinimumNumberOfPlayers = parameters.MinimumNumberOfPlayers;
			MaximumNumberOfPlayers = parameters.MaximumNumberOfPlayers;
			StartDate = parameters.MaxValidStartDate;
		}

		public void AddPlayer(IDictionary<string, object> parameters)
		{
			if (ActualNumberOfPlayers >= MaximumNumberOfPlayers)
			{
				throw new DomainValidationException("No place available on this game.");
			}

		    var playerAddedEvent = new PlayerAddedEvent(parameters);
		    var playerName = playerAddedEvent.RehydratedParameters.PlayerName;

		    if (!AddedPlayersToBoardGame.Contains(playerName))
		    {
                Apply(playerAddedEvent);
		    }
		    else
            {
                throw new DomainValidationException("Player already added to board.");
		    }
		}

        private void OnPlayerAdded(PlayerAddedEvent @event)
        {
            ActualNumberOfPlayers++;
            AddedPlayersToBoardGame.Add(@event.RehydratedParameters.PlayerName);
        }

        public void StartGame(IDictionary<string, object> parameters)
	    {
            var gameStartedEvent = new GameStartedEvent(parameters);

            if (ActualNumberOfPlayers < MinimumNumberOfPlayers)
            {
                throw new DomainValidationException("Not enough players to start the game.");
            }

            if (IsGameStarted)
            {
                throw new DomainValidationException("The game has already started.");
            }
            else
            {
                Apply(gameStartedEvent);
            }


	    }

	    private void OnGameStarted(GameStartedEvent @event)
        {
            IsGameStarted = true;
	        StartDate = @event.RehydratedParameters.StartDate;
        }
	}
}