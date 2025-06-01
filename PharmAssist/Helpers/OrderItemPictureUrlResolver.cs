using AutoMapper;
using PharmAssist.APIs.DTOs;
using PharmAssist.Core.Entities.Order_Aggregation;

namespace PharmAssist.APIs.Helpers
{
	public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDTO, string>
	{
		private readonly IConfiguration _configuration;

		public OrderItemPictureUrlResolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
		{
			if (!string.IsNullOrEmpty(source.Product.PictureUrl))
				return $"{_configuration["ApiBaseUrl"]}{source.Product.PictureUrl}";
			return string.Empty;
		}
	}
}
