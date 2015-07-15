namespace Smi.Sample.AppCore.Player.QueryResults
{
	using System;

	public class PlayerSummaryReporting
	{
		public Guid PlayerId { get; set; }
		public string PlayerName { get; set; }
		public long PlayerScore { get; set; }
	    public int PlayerExperience { get; set; }
        public int PlayerVictory { get; set; }
	}
}