namespace Smi.Sample.Infra.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Linq.Expressions;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	public class EntityRepository : IEntityRepository
	{
		public IList<TEntity> FindAll<TEntity>() where TEntity : class
		{
			return ActiveRecordLinq.AsQueryable<TEntity>().ToList();
		}

		public IList<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			ReadOnlyCollection<ParameterExpression> p = predicate.Parameters;

			return ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).ToList();
		}

		public TEntity FindFirst<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			return ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).FirstOrDefault();
		}

		public IList<TEntity> FindTop<TEntity>(Expression<Func<TEntity, bool>> predicate, int fetchsize) where TEntity : BaseSmiEntityWithSqlId
		{
			Expression<Func<TEntity, long>> orderByExpression = x => x.Id;
			return ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).OrderBy(orderByExpression).Take(fetchsize).ToList();
		}

		public IList<TEntity> FindTopDescending<TEntity>(Expression<Func<TEntity, bool>> predicate, int fetchsize) where TEntity : BaseSmiEntityWithSqlId
		{

			return ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).OrderByDescending(x => x.Id).Take(fetchsize).ToList();
		}

		public void DeleteById<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{

			var entity = ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).FirstOrDefault();
			if (entity == null)
			{
				return;
			}

			ActiveRecordMediator.DeleteAndFlush(entity);
		}

		public void DeleteAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{

			var entities = FindAll(predicate);
			foreach (var entity in entities)
			{
				ActiveRecordMediator.DeleteAndFlush(entity);
			}
		}

		public long Save<TEntity>(TEntity entity, Func<TEntity, long> returnIdCriteria) where TEntity : class
		{

			ActiveRecordMediator.Save(entity);
			return returnIdCriteria(entity);
		}

		public void Save<TEntity>(TEntity entity) where TEntity : class
		{

			ActiveRecordMediator.Save(entity);
		}

		public void SaveAll<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{

			foreach (var entity in entities)
			{
				ActiveRecordMediator.Save(entity);
			}
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{

			ActiveRecordMediator.Update(entity);
		}

		public void UpdateAndFlush<TEntity>(TEntity entity) where TEntity : class
		{

			ActiveRecordMediator.UpdateAndFlush(entity);
		}

		public IList<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate, int startposition, int pageSize) where TEntity : class
		{

			ReadOnlyCollection<ParameterExpression> p = predicate.Parameters;

			return ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).Skip(startposition).Take(pageSize).ToList();
		}

		public IList<TEntity> FindAll<TEntity, TKeySelector>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKeySelector>> keySelector, int startposition, int pageSize) where TEntity : class
		{

			ReadOnlyCollection<ParameterExpression> p = predicate.Parameters;

			return ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).OrderBy(keySelector).Skip(startposition).Take(pageSize).ToList();
		}

		public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{

			ReadOnlyCollection<ParameterExpression> p = predicate.Parameters;

			return ActiveRecordLinq.AsQueryable<TEntity>().Where(predicate).Count();
		}
	}
}