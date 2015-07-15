namespace Smi.Sample.AppCore.Player.Events
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using Smi.Infra.Messages;

    [Serializable]
    public class PlayerExperienceAdded : DomainEvent<IAddPlayerExperience>
    {
        public PlayerExperienceAdded(IDictionary<string, object> parameters)
            : base(parameters)
        { }
    }
}