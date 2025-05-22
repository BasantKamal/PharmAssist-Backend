using PharmAssist.Core.Entities;
using PharmAssist.Core.Specifications;

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

		Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec);

		Task Add(T item);
	}
}
