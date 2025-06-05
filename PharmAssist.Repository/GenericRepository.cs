using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Specifications;
using PharmAssist.Repository.Data;
using System.Linq.Expressions;


namespace PharmAssist.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreContext _dbContext;
        public GenericRepository(StoreContext dbContext)
        {
				_dbContext = dbContext;
        }
		#region Without Specifications
		public async Task<IReadOnlyList<T>> GetAllAsync()
		{
				return await _dbContext.Set<T>().ToListAsync();
		}
	
		public async Task<T> GetByIdAsync(int id)
		{
			return await _dbContext.Set<T>().FindAsync(id);
			  
		}
		#endregion

		#region With Specifications
		public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
		{
			//Set<T>() 3shan tb2a generic
			return await ApplySpecification(Spec).ToListAsync();
		}

		public async Task<T> GetByIdWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).FirstOrDefaultAsync();
		}
		#endregion

		#region Advanced Queries
		public async Task<IReadOnlyList<T>> GetAsync(
			Expression<Func<T, bool>> filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
			string includeProperties = "",
			int? take = null,
			int skip = 0)
		{
			IQueryable<T> query = _dbContext.Set<T>();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProperty in includeProperties.Split
					(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty);
				}
			}

			if (orderBy != null)
			{
				query = orderBy(query);
			}

			if (skip > 0)
			{
				query = query.Skip(skip);
			}

			if (take.HasValue)
			{
				query = query.Take(take.Value);
			}

			return await query.ToListAsync();
		}

		public async Task<T> GetEntityWithSpecAsync(
			Expression<Func<T, bool>> filter,
			string includeProperties = "")
		{
			IQueryable<T> query = _dbContext.Set<T>();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProperty in includeProperties.Split
					(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty);
				}
			}

			return await query.FirstOrDefaultAsync();
		}

		public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
		{
			IQueryable<T> query = _dbContext.Set<T>();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return await query.CountAsync();
		}
		#endregion

		private IQueryable<T> ApplySpecification(ISpecifications<T> Spec)
		{
			return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), Spec);
		}

		public async Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).CountAsync();
		}

		public async Task Add(T item)
			=> await _dbContext.Set<T>().AddAsync(item);

		public void Update(T entity)
			=> _dbContext.Set<T>().Update(entity);

		public void Delete(T entity)
			=> _dbContext.Set<T>().Remove(entity);
		
	}
}
