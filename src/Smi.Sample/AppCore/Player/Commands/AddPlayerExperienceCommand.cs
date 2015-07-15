namespace Smi.Sample.AppCore.Player.Commands
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Smi.Infra.Messages;

    public interface IAddPlayerExperience
    {
        Guid PlayerId { get; set; }
        int ExperienceToAdd { get; set; }
    }

    public class AddPlayerExperienceCommand : BaseCommand, IAddPlayerExperience
    {
        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public int ExperienceToAdd { get; set; }
    }
}