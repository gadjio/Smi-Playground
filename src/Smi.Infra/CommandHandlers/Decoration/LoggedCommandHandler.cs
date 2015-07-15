namespace Smi.Infra.CommandHandlers.Decoration
{
	using System.Diagnostics;
	using Castle.Core.Logging;
	using Handlers;
	using Smi.Infra.Messages;

	public class LoggedCommandHandler<T> : IHandleCommand<T> where T : ICommand
	{
		private readonly IHandleCommand<T> internalCommandHandler;
		private readonly ILogger logger;

		public LoggedCommandHandler(IHandleCommand<T> internalCommandHandler, ILogger logger)
		{
			this.internalCommandHandler = internalCommandHandler;
			this.logger = logger;
		}

		public void Execute(T command)
		{
			Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();

			logger.Info(string.Format("Command handling for command {0} started.", typeof(T)));

			internalCommandHandler.Execute(command);

			stopWatch.Stop();
			logger.Info(string.Format("Command handling for command {0} ended. Took {1} ms.", typeof(T), stopWatch.Elapsed.TotalSeconds));
		}

		public void OnFail(T command)
		{

		}
	}
}