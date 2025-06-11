using PharmAssist.Core.Entities;

namespace PharmAssist.Core.Specifications
{
    public class ProductSearchWithFilterationForCountAsync : BaseSpecifications<Product>
    {
        public ProductSearchWithFilterationForCountAsync(SearchProductsParam searchParams)
            : base(p =>
                // General query parameter to search across multiple fields
                (string.IsNullOrEmpty(searchParams.Query) || 
                    p.Name.ToLower().Contains(searchParams.Query) || 
                    p.Description.ToLower().Contains(searchParams.Query) || 
                    p.ActiveIngredient.ToLower().Contains(searchParams.Query) ||
                    (p.Conflicts != null && p.Conflicts.ToLower().Contains(searchParams.Query))) &&
                
                // Individual field filters
                (string.IsNullOrEmpty(searchParams.Name) || p.Name.ToLower().Contains(searchParams.Name)) &&
                (string.IsNullOrEmpty(searchParams.Description) || p.Description.ToLower().Contains(searchParams.Description)) &&
                (string.IsNullOrEmpty(searchParams.ActiveIngredient) || p.ActiveIngredient.ToLower().Contains(searchParams.ActiveIngredient)) &&
                
                // Price range filters
                (!searchParams.MinPrice.HasValue || p.Price >= searchParams.MinPrice) &&
                (!searchParams.MaxPrice.HasValue || p.Price <= searchParams.MaxPrice)
            )
        {
            // No need for sorting or pagination in count specification
        }
    }
} 