namespace Smi.Sample.AppCore.Player.Commands
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Smi.Infra.Messages;

    public interface IAddPlayerVictories
    {
        Guid PlayerId { get; set; }
        int VictoriesToAdd { get; set; }
        int ExperienceToAdd { get; set; }
    }

    public class AddPlayerVictoriesCommand : BaseCommand, IAddPlayerVictories
    {
        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public int VictoriesToAdd { get; set; }

        [Required]
        public int ExperienceToAdd { get; set; }

    }
}