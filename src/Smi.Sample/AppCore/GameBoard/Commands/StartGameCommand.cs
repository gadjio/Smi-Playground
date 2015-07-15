namespace Smi.Sample.AppCore.GameBoard.Commands
{
	using System;
	using Smi.Infra.Messages;

	public interface IStartGame
	{
		DateTime StartDate { get; set; }
	}

	public class StartGameCommand : BaseDomainCommand, IStartGame
	{
		public DateTime StartDate { get; set; }
	}
}