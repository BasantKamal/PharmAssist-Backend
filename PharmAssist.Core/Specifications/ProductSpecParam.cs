

namespace PharmAssist.Core.Specifications
{
	public class ProductSpecParam
	{
		public string? Sort { get; set; }

		private int pageSize = 10;

		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value > 10 ? 10 : value; }
		}
		public int PageIndex { get; set; } = 1;


		private string? activeIngredient;
		public string? ActiveIngredient
		{
			get { return activeIngredient; }
			set { activeIngredient = value?.ToLower(); }
		}

		private string? search;
		public string? Search
		{
			get { return search; }
			set { search = value.ToLower(); }
		}


	}
}
