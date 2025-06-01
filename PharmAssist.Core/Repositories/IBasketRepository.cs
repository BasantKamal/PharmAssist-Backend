using PharmAssist.Core.Entities;

namespace PharmAssist.Core.Repositories
{
	public interface IBasketRepository
	{
		 Task<CustomerBasket?> GetBasketAsync(string basketId);
		Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket);
		Task<bool> DeleteBasketAsync(string basketId);
	}
}
