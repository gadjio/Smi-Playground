namespace Smi.Infra.CommandHandlers.Decoration
{
	using Castle.ActiveRecord;
	using Handlers;
	using Smi.Infra.Messages;

	public class TransactionalCommandHandler<T> : IHandleCommand<T> where T : ICommand
	{
		private readonly IHandleCommand<T> internalCommandHandler;		

		public TransactionalCommandHandler(IHandleCommand<T> internalCommandHandler)
		{
			this.internalCommandHandler = internalCommandHandler;			
		}

		protected virtual System.Data.IsolationLevel GetIsolationLevel()
		{
			return System.Data.IsolationLevel.ReadCommitted;
		}

		public void Execute(T command)
		{
			if(!ActiveRecordStarter.IsInitialized)
			{
				internalCommandHandler.Execute(command);
				return;
			}

			using (var trx = new Castle.ActiveRecord.TransactionScope(TransactionMode.Inherits, GetIsolationLevel(), OnDispose.Commit))
			{
				try
				{
					internalCommandHandler.Execute(command);
					trx.VoteCommit();
				}
				catch
				{
					trx.VoteRollBack();
					throw;
				}
			}			
		}

		public void OnFail(T command)
		{		
			internalCommandHandler.OnFail(command);
		}
	}


}
