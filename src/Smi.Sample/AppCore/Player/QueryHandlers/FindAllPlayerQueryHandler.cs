namespace Smi.Sample.AppCore.Player.QueryHandlers
{
	using System.Collections.Generic;
	using System.Linq;
	using Entities;
	using Infra.Entities;
	using Queries;
	using QueryResults;
	using Services;
	using Smi.Infra.Handlers;

	public class FindAllPlayerQueryHandler : IHandleQuery<FindAllPlayerQuery, IEnumerable<PlayerSummaryReporting>>
	{
		private readonly IEntityRepository entityRepository;
		private readonly IPlayerSummaryReportingBuilder builder;

		public FindAllPlayerQueryHandler(IEntityRepository entityRepository, IPlayerSummaryReportingBuilder builder)
		{
			this.entityRepository = entityRepository;
			this.builder = builder;
		}

		public IEnumerable<PlayerSummaryReporting> Handle(FindAllPlayerQuery query)
		{
			var result = new List<PlayerSummaryReporting>();

			var players = entityRepository.FindAll<PlayerEntity>().ToList();
			foreach (var player in players)
			{				
				var score = entityRepository.FindFirst<PlayerScore>(x => x.AggregateRootId == player.AggregateRootId);
                var experience = entityRepository.FindFirst<PlayerExperience>(x => x.AggregateRootId == player.AggregateRootId);
                var victory = entityRepository.FindFirst<PlayerVictory>(x => x.AggregateRootId == player.AggregateRootId);	
				result.Add(builder.Create(player, score, experience, victory));
			}
			return result;
		}
	}
}