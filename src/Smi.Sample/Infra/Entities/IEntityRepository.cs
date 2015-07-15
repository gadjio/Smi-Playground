namespace Smi.Sample.Infra.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	public interface IEntityRepository
	{
		IList<TEntity> FindAll<TEntity>() where TEntity : class;
		IList<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
		IList<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate, int startposition, int pageSize) where TEntity : class;
		IList<TEntity> FindAll<TEntity, TKeySelector>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKeySelector>> keySelector, int startposition, int pageSize) where TEntity : class;

		TEntity FindFirst<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

		void DeleteById<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
		void DeleteAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
		long Save<TEntity>(TEntity entity, Func<TEntity, long> returnIdCriteria) where TEntity : class;
		void Save<TEntity>(TEntity entity) where TEntity : class;
		void SaveAll<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		void Update<TEntity>(TEntity entity) where TEntity : class;
		void UpdateAndFlush<TEntity>(TEntity entity) where TEntity : class;

		int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

		IList<TEntity> FindTop<TEntity>(Expression<Func<TEntity, bool>> predicate, int fetchsize) where TEntity : BaseSmiEntityWithSqlId;
		IList<TEntity> FindTopDescending<TEntity>(Expression<Func<TEntity, bool>> predicate, int fetchsize) where TEntity : BaseSmiEntityWithSqlId;
	}
}