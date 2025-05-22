

using PharmAssist.Core.Entities;

namespace PharmAssist.Core.Specifications
{
	public class ProductSpecs:BaseSpecifications<Product>
	{
        //CTOR used to GET all products
        public ProductSpecs(ProductSpecParam Params) 
            :base(p=>
            (string.IsNullOrEmpty(Params.Search) || p.Name.ToLower().Contains(Params.Search)))
        {
          
            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch(Params.Sort)
                {
                    case "PriceAsc":
                        AddOrderBy(p => p.Price);
                            break;
                    case "PriceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
					case "ActiveIngredient":
						AddOrderBy(p => p.ActiveIngredient);
						break;
					default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            //products = 100
            //page Size = 10
            //page index = 5

            //SKIP => 40 =10 * 4
            //TAKE => 10
         
            ApplyPagination(Params.PageSize * (Params.PageIndex - 1), Params.PageSize);
		}

        //CTOR used to GET product by id
        public ProductSpecs(int id):base(p=>p.Id==id)
        {
			
		}
    }
}
