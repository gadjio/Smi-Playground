namespace Smi.Sample.AppCore.Player.Services
{
	using Entities;
	using QueryResults;

	public interface IPlayerSummaryReportingBuilder
	{
        PlayerSummaryReporting Create(PlayerEntity player, PlayerScore score, PlayerExperience experience, PlayerVictory victory);
	}

	public class PlayerSummaryReportingBuilder : IPlayerSummaryReportingBuilder
	{
        public PlayerSummaryReporting Create(PlayerEntity player, PlayerScore score, PlayerExperience experience, PlayerVictory victory)
		{
			var item = new PlayerSummaryReporting { PlayerName = player.Name, PlayerId = player.AggregateRootId, PlayerExperience = experience.Experience, PlayerVictory = victory.NumberOfVictory };
			
			if (score != null)
			{
				item.PlayerScore = score.Score;
			}

			return item;
		}
	}
}