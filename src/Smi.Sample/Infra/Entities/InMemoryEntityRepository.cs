namespace Smi.Sample.Infra.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	public class InMemoryEntityRepository : IEntityRepository
	{
		private IDictionary<Type, IList<object>> inMemoryMap = new Dictionary<Type, IList<object>>();

		public IList<TEntity> FindAll<TEntity>(Func<IQueryable<TEntity>, IEnumerable<TEntity>> criteria) where TEntity : class
		{
			var key = typeof(TEntity);
			if (!inMemoryMap.ContainsKey(key))
			{
				return new List<TEntity>();
			}

			return criteria(inMemoryMap[key].Cast<TEntity>().AsQueryable()).ToList();
		}


		public TEntity FindFirst<TEntity>(Func<IQueryable<TEntity>, IEnumerable<TEntity>> criteria) where TEntity : class
		{
			return FindAll(criteria).FirstOrDefault();
		}


		public IList<TEntity> FindAll<TEntity>() where TEntity : class
		{
			var key = typeof(TEntity);
			if (!inMemoryMap.ContainsKey(key))
			{
				return new List<TEntity>();
			}

			return inMemoryMap[key].Cast<TEntity>().ToList();
		}

		public IList<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			var key = typeof(TEntity);
			if (!inMemoryMap.ContainsKey(key))
			{
				return new List<TEntity>();
			}

			var query = predicate.Compile();

			return inMemoryMap[key].Cast<TEntity>().Where(query).ToList();
		}

		public IList<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate, int startposition, int pageSize) where TEntity : class
		{
			throw new NotImplementedException();
		}

		public IList<TEntity> FindAll<TEntity, TKeySelector>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKeySelector>> keySelector, int startposition, int pageSize) where TEntity : class
		{
			throw new NotImplementedException();
		}


		public IList<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, int>> order, int startposition, int pageSize) where TEntity : class
		{
			throw new NotImplementedException();
		}

		public TEntity FindFirst<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			return FindAll(predicate).FirstOrDefault();
		}

		public void DeleteById<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			var key = typeof(TEntity);
			var query = predicate.Compile();

			var itemToRemove = inMemoryMap[key].Cast<TEntity>().Where(query).Single();
			inMemoryMap[key].Remove(itemToRemove);
		}

		public void DeleteAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			var key = typeof(TEntity);
			var query = predicate.Compile();

			var itemsToRemove = inMemoryMap[key].Cast<TEntity>().Where(query).ToList();
			foreach (var itemToRemove in itemsToRemove)
			{
				inMemoryMap[key].Remove(itemToRemove);
			}
		}

		public long Save<TEntity>(TEntity entity, Func<TEntity, long> returnIdCriteria) where TEntity : class
		{
			var key = typeof(TEntity);
			if (!inMemoryMap.ContainsKey(key))
			{
				inMemoryMap.Add(key, new List<object>());
			}

			inMemoryMap[key].Add(entity);

			return inMemoryMap[key].Count;
		}

		public void Save<TEntity>(TEntity entity) where TEntity : class
		{
			var key = typeof(TEntity);
			if (!inMemoryMap.ContainsKey(key))
			{
				inMemoryMap.Add(key, new List<object>());
			}

			inMemoryMap[key].Add(entity);
		}

		public void SaveAll<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			foreach (var entity in entities)
			{
				Save(entity);
			}
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
		}

		public void UpdateAndFlush<TEntity>(TEntity entity) where TEntity : class
		{
		}

		public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
		{
			var items = this.FindAll<TEntity>(predicate);
			return items.Count;
		}

		public IList<TEntity> FindTop<TEntity>(Expression<Func<TEntity, bool>> predicate, int fetchsize) where TEntity : BaseSmiEntityWithSqlId
		{
			var items = this.FindAll<TEntity>(predicate);
			return items.Take(fetchsize).ToList();
		}

		public IList<TEntity> FindTopDescending<TEntity>(Expression<Func<TEntity, bool>> predicate, int fetchsize) where TEntity : BaseSmiEntityWithSqlId
		{
			throw new NotImplementedException();
		}
	}
}