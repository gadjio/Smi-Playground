namespace Smi.Infra.CommandHandlers
{
	using System.Linq;
	using Castle.MicroKernel.Facilities;
	using Handlers;

	public class CommandHandlerDecorationFacility : AbstractFacility
	{
		protected override void Init()
		{
			Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
		}

		static void Kernel_ComponentModelCreated(Castle.Core.ComponentModel model)
		{
			if (model.Services.First().IsGenericType && model.Services.First().GetGenericTypeDefinition().Equals(typeof(IHandleCommand<>)))
			{
				model.CustomComponentActivator = typeof(CommandHandlerComponentActivator);
			}
		}
	}
}