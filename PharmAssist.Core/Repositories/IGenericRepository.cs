using PharmAssist.Core.Entities;
using PharmAssist.Core.Specifications;
using System.Linq.Expressions;

namespace PharmAssist.Core.Repositories
{
	public interface IGenericRepository<T> where T : BaseEntity
	{
		#region Without Specifications
		Task<IReadOnlyList<T>> GetAllAsync();
		Task<T> GetByIdAsync(int id);
		#endregion

		#region With Specifications
		Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec);
		Task<T> GetByIdWithSpecAsync (ISpecifications<T> Spec);
		#endregion

		#region Advanced Queries
		Task<IReadOnlyList<T>> GetAsync(
			Expression<Func<T, bool>> filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
			string includeProperties = "",
			int? take = null,
			int skip = 0);
		
		Task<T> GetEntityWithSpecAsync(
			Expression<Func<T, bool>> filter,
			string includeProperties = "");
		
		Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
		#endregion

		Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec);

		Task Add(T item);
		void Update(T entity);
		void Delete(T entity);
	}
}
