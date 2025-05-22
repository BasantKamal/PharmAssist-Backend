using AutoMapper;
using PharmAssist.APIs.DTOs;
using PharmAssist.Core.Entities;


namespace PharmAssist.APIs.Helpers
{
	public class ProductPictureUrlResolver : IValueResolver<Product, ProductToReturnDTO, string>
	{
		private readonly IConfiguration _configuration;

		public ProductPictureUrlResolver(IConfiguration configuration)
        {
			_configuration = configuration;
		}
        public string Resolve(Product source, ProductToReturnDTO destination, string destMember, ResolutionContext context)
		{
			if (!string.IsNullOrEmpty(source.PictureUrl))
				return $"{_configuration["ApiBaseUrl"]}{source.PictureUrl}";
			return string.Empty ;	
			
		}
	}
}
