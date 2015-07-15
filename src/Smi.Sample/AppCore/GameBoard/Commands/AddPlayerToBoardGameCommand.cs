namespace Smi.Sample.AppCore.GameBoard.Commands
{
	using System;
	using Smi.Infra.Messages;

	public interface IAddPlayerToBoardGame
	{
		Guid PlayerId { get; set; }
		DateTime Date { get; set; }
		string PlayerName { get; set; }
	}

	public class AddPlayerToBoardGameCommand : BaseDomainCommand, IAddPlayerToBoardGame
	{
		public Guid PlayerId { get; set; }
		public DateTime Date { get; set; }

		public string PlayerName { get; set; }
	}
}