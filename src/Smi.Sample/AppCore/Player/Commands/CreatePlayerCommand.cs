namespace Smi.Sample.AppCore.Player.Commands
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using Smi.Infra.Messages;

	public interface ICreatePlayer
	{
		Guid PlayerId { get; set; }
		string PlayerName { get; set; }
	}

	public class CreatePlayerCommand : BaseCommand, ICreatePlayer
	{

		[Required]
		public Guid PlayerId { get; set; }

		[Required]
		public string PlayerName { get; set; }

	}
}