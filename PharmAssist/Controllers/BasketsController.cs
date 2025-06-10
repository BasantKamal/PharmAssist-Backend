using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.APIs.DTOs;
using PharmAssist.Controllers;
using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Specifications;
using PharmAssist.Errors;


namespace PharmAssist.APIs.Controllers
{
	public class BasketsController : APIBaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public BasketsController(IBasketRepository basketRepository,IMapper mapper, IUnitOfWork unitOfWork)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		[HttpGet("test-redis")]
		public async Task<ActionResult> TestRedisConnection()
		{
			try
			{
				var testBasket = new CustomerBasket("test-connection");
				testBasket.Items = new List<BasketItem>();
				
				var result = await _basketRepository.UpdateBasketAsync(testBasket);
				if (result != null)
				{
					await _basketRepository.DeleteBasketAsync("test-connection");
					return Ok(new { message = "Redis connection successful!", timestamp = DateTime.UtcNow });
				}
				return BadRequest(new { message = "Failed to create test basket" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "Redis connection failed", error = ex.Message });
			}
		}

		//GET Or ReCreate Basket
		[HttpGet]
		public async Task<ActionResult<CustomerBasket>>GetCustomerBasket(string basketId)
		{
			var basket=await _basketRepository.GetBasketAsync(basketId);
			
			return basket is null ? new CustomerBasket(basketId) : basket; //recreate ya3ny lw msh mwgoda hthotha tany
		}


		[HttpPost("AddProduct")]
		public async Task<ActionResult<CustomerBasketDTO>> AddProductToCart(string basketId,int productId)
		{
			var product = await _unitOfWork.Repository<Product>()
				.GetByIdWithSpecAsync(new ProductSpecs(productId));
			if (product == null)
				return NotFound(new ApiResponse(404, "Product not found"));

			var basket = await _basketRepository.GetBasketAsync(basketId)
						 ?? new CustomerBasket(basketId);
			basket.Items ??= new List<BasketItem>();

			var item = basket.Items.FirstOrDefault(i => i.Id == productId);
			if (item != null)
			{
				item.Quantity++;
				item.Price = product.Price;
				item.Name = product.Name;
				item.PictureUrl = product.PictureUrl;
				item.ActiveIngredient = product.ActiveIngredient;
			}
			else
			{
				basket.Items.Add(new BasketItem
				{
					Id = productId,
					Quantity = 1,
					Price = product.Price,
					Name = product.Name,
					PictureUrl = product.PictureUrl,
					ActiveIngredient = product.ActiveIngredient
				});
			}

			var updated = await _basketRepository.UpdateBasketAsync(basket);
			if (updated == null)
				return BadRequest(new ApiResponse(400, "Failed to update basket"));

			var dto = _mapper.Map<CustomerBasketDTO>(updated);
			return Ok(dto);
		}

		
		[HttpDelete("RemoveProduct")]
		public async Task<ActionResult<CustomerBasketDTO>> RemoveProductFromCart(string basketId, int productId)
		{
			var basket = await _basketRepository.GetBasketAsync(basketId);
			if (basket == null) return NotFound(new ApiResponse(404, "Basket not found"));

			var item = basket.Items.FirstOrDefault(i => i.Id == productId);
			if (item == null) return NotFound(new ApiResponse(404, "Product not found in basket"));

			basket.Items.Remove(item);

			var updated = await _basketRepository.UpdateBasketAsync(basket);
			if (updated == null) return BadRequest(new ApiResponse(400, "Failed to update basket"));

			var dto = _mapper.Map<CustomerBasketDTO>(updated);
			return Ok(dto);
		}

		//Update Or Create new basket
		[HttpPost]
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDTO basket)
		{
			var mappedBasket = _mapper.Map<CustomerBasketDTO, CustomerBasket>(basket);
			var CreatedOrUpdatedBasket= await _basketRepository.UpdateBasketAsync(mappedBasket);
			if(CreatedOrUpdatedBasket is null) return BadRequest(new ApiResponse(400)); 
			return Ok(CreatedOrUpdatedBasket);
		}

		[HttpDelete]
		public async Task<ActionResult<bool>> DeleteBasket(string basketId)
		{
			return await _basketRepository.DeleteBasketAsync(basketId);
		}
	}
}
