
using PharmAssist.Core.Entities;

namespace PharmAssist.Core.Specifications
{
	public class ProductWithFilterationForCountAsync : BaseSpecifications<Product>
	{
        public ProductWithFilterationForCountAsync(ProductSpecParam Params):base(p=> 
		(string.IsNullOrEmpty(Params.Search) || p.Name.ToLower().Contains(Params.Search)))
        {
			
        }
    }
}
