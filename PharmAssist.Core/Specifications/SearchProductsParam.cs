namespace PharmAssist.Core.Specifications
{
    public class SearchProductsParam
    {
        public string? Sort { get; set; }

        private int pageSize = int.MaxValue;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }
        public int PageIndex { get; set; } = 1;

        private string? query;
        public string? Query
        {
            get { return query; }
            set { query = value?.ToLower(); }
        }

        private string? name;
        public string? Name
        {
            get { return name; }
            set { name = value?.ToLower(); }
        }

        private string? description;
        public string? Description
        {
            get { return description; }
            set { description = value?.ToLower(); }
        }

        private string? activeIngredient;
        public string? ActiveIngredient
        {
            get { return activeIngredient; }
            set { activeIngredient = value?.ToLower(); }
        }

        private decimal? minPrice;
        public decimal? MinPrice
        {
            get { return minPrice; }
            set { minPrice = value; }
        }

        private decimal? maxPrice;
        public decimal? MaxPrice
        {
            get { return maxPrice; }
            set { maxPrice = value; }
        }
    }
} 