namespace Smi.Infra.CommandHandlers
{
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Context;

	public class CommandHandlerComponentActivator : DefaultComponentActivator
	{
		private readonly CommandHandlerChainBuilder chainBuilder;

		public CommandHandlerComponentActivator(ComponentModel model, IKernelInternal kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
			chainBuilder = new CommandHandlerChainBuilder(kernel);
		}

		protected override object CreateInstance(CreationContext context, ConstructorCandidate constructor, object[] arguments)
		{
			var commandHandler = base.CreateInstance(context, constructor, arguments);

			var result = chainBuilder.BuildChain(commandHandler);
			return result;
		}

	}
}