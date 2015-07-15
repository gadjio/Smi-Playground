namespace Smi.Sample.Infra.Model
{
	using System;
	using Castle.ActiveRecord;
	using Entities;

	[ActiveRecord]
	public class EventLog : BaseSmiEntity
	{
		[PrimaryKey(PrimaryKeyType.Identity)]
		public long Id { get; set; }

		[Property]
		public Guid? EventId { get; set; }

		[Property]
		public Guid? EventProviderId { get; set; }

		[Property]
		public int? Version { get; set; }

		[Property]
		public string Type { get; set; }

		[Property("[User]")]
		public string User { get; set; }

		[Property]
		public DateTime? DateHappened { get; set; }

		[Property(Length = 8001)]
		public string JSonDomainEvent { get; set; }

	}
}