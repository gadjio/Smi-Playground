namespace Smi.Infra.EventStore.Aggregate
{
	using System;

	[Serializable]
	public class UnregisteredDomainEventException : Exception
	{
		public UnregisteredDomainEventException(string message) : base(message) { }
	}
}