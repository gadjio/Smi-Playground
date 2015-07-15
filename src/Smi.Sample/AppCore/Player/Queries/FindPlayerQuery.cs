namespace Smi.Sample.AppCore.Player.Queries
{
	using System;
	using QueryResults;
	using Smi.Infra.Messages;

	public class FindPlayerQuery : IQuery<PlayerSummaryReporting>
	{
		public string Email { get; set; }
		public Guid? PlayerId { get; set; }

		public FindPlayerQuery(Guid playerId)
		{
			PlayerId = playerId;
		}

		public FindPlayerQuery(string email)
		{
			Email = email;
		}
	}
}