

using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;

namespace PharmAssist.Core
{
	public interface IUnitOfWork:IAsyncDisposable
	{
		IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
		Task<int> CompleteAsync();
	}
}
