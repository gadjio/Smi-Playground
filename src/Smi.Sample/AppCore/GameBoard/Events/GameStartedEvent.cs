namespace Smi.Sample.AppCore.GameBoard.Events
{
	using System;
	using System.Collections.Generic;
	using Commands;
	using Smi.Infra.Messages;

	[Serializable]
	public class GameStartedEvent : DomainEvent<IStartGame>
	{
        public GameStartedEvent(IDictionary<string, object> parameters)
			: base(parameters)
		{ }
	}
}