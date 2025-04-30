﻿using AutoMapper;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.DTOs;


namespace PharmAssist.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<AppUser, UserProfileDto>();
			CreateMap<UserProfileDto, AppUser>()
				.ForMember(dest => dest.Id, opt => opt.Ignore());

			//CreateMap<Product, ProductToReturnDTO>()
			//	.ForMember(d=>d.ProductType,o=>o.MapFrom(s=>s.ProductType.Name))
			//	.ForMember(d=>d.ProductBrand,o=>o.MapFrom(s=>s.ProductBrand.Name))
			//	.ForMember(d => d.PictureUrl,o=>o.MapFrom<ProductPictureUrlResolver>());

			CreateMap<Core.Entities.Identity.Address, AddressDTO>().ReverseMap(); 

			

			//CreateMap<AddressDTO,Core.Entities.Order_Aggregation.Address>();
			//CreateMap<CustomerBasketDTO, CustomerBasket>();
			//CreateMap<BasketItemDTO, BasketItem>();
			//CreateMap<Order, OrderToReturnDTO>()
			//		 .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
			//		 .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));
			//CreateMap<OrderItem, OrderItemDTO>()
			//	.ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
			//	.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
			//	.ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl))
			//	.ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());
		}
	}
}
