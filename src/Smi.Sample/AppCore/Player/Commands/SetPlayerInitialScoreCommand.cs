namespace Smi.Sample.AppCore.Player.Commands
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using Smi.Infra.Messages;

	public interface ISetPlayerInitialScore
	{
		Guid PlayerId { get; set; }
		
		int InitialScore { get; set; }
	}

	public class SetPlayerInitialScoreCommand : BaseCommand, ISetPlayerInitialScore
	{
		[Required]
		public Guid PlayerId { get; set; }

		[Required]
		public int InitialScore { get; set; }
	}
}