namespace Smi.Sample.Infra.Entities
{
	using Castle.ActiveRecord;

	public abstract class BaseSmiEntity
	{
	}

	public abstract class BaseSmiEntityWithSqlId : BaseSmiEntity
	{
		[PrimaryKey(PrimaryKeyType.Identity)]
		public long Id { get; set; }
	}
}