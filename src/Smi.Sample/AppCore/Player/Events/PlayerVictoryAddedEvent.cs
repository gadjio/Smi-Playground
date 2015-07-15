namespace Smi.Sample.AppCore.Player.Events
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using Smi.Infra.Messages;

    [Serializable]
    public class PlayerVictoryAdded : DomainEvent<IAddPlayerVictories>
    {
        public PlayerVictoryAdded(IDictionary<string, object> parameters)
            : base(parameters)
        { }
    }
}