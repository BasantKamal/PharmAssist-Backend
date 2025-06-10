using AutoMapper;
using PharmAssist.APIs.DTOs;
using PharmAssist.Core.Entities;

namespace PharmAssist.Helpers
{
	public class BasketItemPictureUrlResolver: IValueResolver<BasketItem, BasketItemDTO, string>
	{
		private readonly IConfiguration _configuration;

		public BasketItemPictureUrlResolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string Resolve(BasketItem source, BasketItemDTO destination, string destMember, ResolutionContext context)
		{
			if (!string.IsNullOrEmpty(source.PictureUrl))
				return $"{_configuration["ApiBaseUrl"]}{source.PictureUrl}";
			return string.Empty;

		}
	}
}
