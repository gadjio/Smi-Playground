namespace Smi.Infra.Handlers
{
	using Smi.Infra.Messages;

	public interface IHandleCommand<T> where T : ICommand
	{
		void Execute(T command);
		void OnFail(T command);
	}
}