namespace Smi.Sample.Infra.Services
{
	using System.Collections.Generic;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using NHibernate;
	using NHibernate.Dialect;
	using NHibernate.Engine;
	using NHibernate.Id;
	using Sample.Infra.Entities;

	public interface IHiLoGenerator
	{
		long GetIdentifier();
	}


	public class HiLoGeneratorNHibernate : IHiLoGenerator
	{
		private readonly ISessionFactoryHolder getSessionFactoryHolder;
		private readonly TableHiLoGenerator generator;

		public HiLoGeneratorNHibernate()
		{
			this.getSessionFactoryHolder = ActiveRecordMediator.GetSessionFactoryHolder();
			generator = new TableHiLoGenerator();
			var configuration = getSessionFactoryHolder.GetConfiguration(typeof(BaseSmiEntity));
			generator.Configure(NHibernateUtil.Int64, new Dictionary<string, string> { { "table", "hibernate_unique_key" } }, Dialect.GetDialect());
		}

		public long GetIdentifier()
		{
			var session = getSessionFactoryHolder.CreateSession(typeof(BaseSmiEntity));
			var implementor = (ISessionImplementor)session;
			return (long)generator.Generate(implementor, null);
		}
	}


	/*
	 * From http://joseoncode.com/2011/03/23/hilo-for-entityframework/
	 */
	//public class HiLoGenerator<TDbContext> : IHiLoGenerator
	//where TDbContext : DbContext, new()
	//{
	//	private static readonly object ConcurrencyLock = new object();
	//	private readonly int maxLo = 1000;
	//	private string connectionString;
	//	private long currentHi = -1;
	//	private int currentLo;

	//	public long GetIdentifier()
	//	{
	//		long result;
	//		lock (ConcurrencyLock)
	//		{
	//			if (currentHi == -1)
	//			{
	//				MoveNextHi();
	//			}
	//			if (currentLo == maxLo)
	//			{
	//				currentLo = 0;
	//				MoveNextHi();
	//			}
	//			result = (currentHi * maxLo) + currentLo;
	//			currentLo++;
	//		}
	//		return result;
	//	}



	//	private void MoveNextHi()
	//	{
	//		using (var connection = CreateConnection())
	//		{
	//			connection.Open();
	//			using (var tx = connection.BeginTransaction(IsolationLevel.Serializable))
	//			using (var selectCommand = connection.CreateCommand())
	//			{
	//				selectCommand.Transaction = tx;
	//				selectCommand.CommandText =
	//					"SELECT next_hi FROM entityframework_unique_key;" +
	//					 "UPDATE entityframework_unique_key SET next_hi = next_hi + 1;";
	//				var result = selectCommand.ExecuteScalar();
	//				currentHi = (int)result;

	//				tx.Commit();
	//			}
	//			connection.Close();
	//		}

	//	}

	//	private IDbConnection CreateConnection()
	//	{
	//		using (var dbContext = new TDbContext())
	//		{
	//			var connectionType = dbContext.Database.Connection.GetType();
	//			connectionString = connectionString ?? dbContext.Database.Connection.ConnectionString;
	//			return (IDbConnection)Activator.CreateInstance(connectionType, connectionString);
	//		}
	//	}
	//}
}