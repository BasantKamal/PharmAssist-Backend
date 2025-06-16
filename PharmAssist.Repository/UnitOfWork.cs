using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using PharmAssist.Repository.Data;
using System.Collections;


namespace PharmAssist.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbcontext;
        private Hashtable _repositories;

        public UnitOfWork(StoreContext dbcontext)
        {
            _dbcontext = dbcontext;
            _repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
        => await _dbcontext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
        => await _dbcontext.DisposeAsync();


        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            //key :product,order
            //value:generic repository<product>
            var type = typeof(TEntity).Name; // ex:product,order
            if (!_repositories.ContainsKey(type))
            {
                IGenericRepository<TEntity> repository;
                
                // Special case for MedicationRecommendation to return the specific repository
                if (type == nameof(MedicationRecommendation))
                {
                    repository = (IGenericRepository<TEntity>)new MedicationRecommendationRepository(_dbcontext);
                }
                else
                {
                    repository = new GenericRepository<TEntity>(_dbcontext);
                }
                
                _repositories.Add(type, repository);
            }
            return _repositories[type] as IGenericRepository<TEntity>;
        }
    }
}
