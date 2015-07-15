namespace Smi.Sample.AppCore.GameBoard.Commands
{
	using System;
	using Smi.Infra.Messages;

	public interface ICreateGameBoard
	{
		string FriendlyName { get; set; }
		int MinimumNumberOfPlayers { get; set; }
		int MaximumNumberOfPlayers { get; set; }
		DateTime MaxValidStartDate { get; set; }
	}

	public class CreateGameBoardCommand :BaseDomainCommand, ICreateGameBoard
	{
		public string FriendlyName { get; set; }
		public int MinimumNumberOfPlayers { get; set; }
		public int MaximumNumberOfPlayers { get; set; }

		public DateTime MaxValidStartDate { get; set; }
	}
}