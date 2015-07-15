namespace Smi.Sample.Entities
{
	using System;

	public class BoardEntity
	{
		public Guid AggregateRootId { get; set; }
	}

	public class PlayerEntity
	{
		public Guid AggregateRootId { get; set; }
		public string Name { get; set; }
		
		//TODO :n 2015-02-09 - Ask for email on Creation
		public string Email { get; set; }
	    public int Experience { get; set; }
	}
}