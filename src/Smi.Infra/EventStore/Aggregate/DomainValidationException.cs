namespace Smi.Infra.EventStore.Aggregate
{
	using System;

	[Serializable]
	public class DomainValidationException : Exception
	{
		public DomainValidationException(string message) : base(message) { }


	}
}