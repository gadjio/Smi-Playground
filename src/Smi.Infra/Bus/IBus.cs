namespace Smi.Infra.Bus
{
	using System.Collections.Generic;
	using Messages;

	public interface IBus
	{
		void Publish<T>(T @event) where T : class, IEvent;
		void Publish<T>(IEnumerable<T> events) where T : class, IEvent;
		void Send(ICommand command);
	}
}