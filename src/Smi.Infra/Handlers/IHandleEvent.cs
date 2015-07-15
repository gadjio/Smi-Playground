namespace Smi.Infra.Handlers
{
	using Smi.Infra.Messages;

	public interface IHandleEvent<T> where T : IEvent
	{
		void Handle(T @event);
	}
}