using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Specifications;
using PharmAssist.Repository.Data;


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

		public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
		{
			//Set<T>() 3shan tb2a generic
			return await ApplySpecification(Spec).ToListAsync();
		}


		public async Task<T> GetByIdWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).FirstOrDefaultAsync();
		}
		

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
		
	}
}
