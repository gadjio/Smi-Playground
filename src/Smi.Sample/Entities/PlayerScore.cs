namespace Smi.Sample.Entities
{
	using System;

	public class PlayerScore
	{
		public Guid AggregateRootId { get; set; }
		public long Score { get; set; }
	}
}