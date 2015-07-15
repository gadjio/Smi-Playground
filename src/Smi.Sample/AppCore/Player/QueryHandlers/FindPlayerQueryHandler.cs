namespace Smi.Sample.AppCore.Player.QueryHandlers
{
	using Entities;
	using Infra.Entities;
	using Queries;
	using QueryResults;
	using Services;
	using Smi.Infra.Handlers;

	public class FindPlayerQueryHandler : IHandleQuery<FindPlayerQuery, PlayerSummaryReporting>
	{
		private readonly IEntityRepository entityRepository;
		private readonly IPlayerSummaryReportingBuilder builder;

		public FindPlayerQueryHandler(IEntityRepository entityRepository, IPlayerSummaryReportingBuilder builder)
		{
			this.entityRepository = entityRepository;
			this.builder = builder;
		}


		public PlayerSummaryReporting Handle(FindPlayerQuery query)
		{			
			PlayerEntity player;

			if (query.PlayerId.HasValue)
			{
				player = entityRepository.FindFirst<PlayerEntity>(x => x.AggregateRootId == query.PlayerId);
			}
			else
			{
				player = entityRepository.FindFirst<PlayerEntity>(x => x.Email == query.Email);
			}

            var score = entityRepository.FindFirst<PlayerScore>(x => x.AggregateRootId == player.AggregateRootId);
            var experience = entityRepository.FindFirst<PlayerExperience>(x => x.AggregateRootId == player.AggregateRootId);
            var victory = entityRepository.FindFirst<PlayerVictory>(x => x.AggregateRootId == player.AggregateRootId);
			return builder.Create(player, score, experience, victory);
		}
	}
}