using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.APIs.DTOs;
using PharmAssist.Controllers;
using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Specifications;
using PharmAssist.Errors;


namespace PharmAssist.APIs.Controllers
{
	[Authorize]
	public class BasketsController : APIBaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<AppUser> _userManager;

		public BasketsController(
			IBasketRepository basketRepository,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			UserManager<AppUser> userManager)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<ActionResult<CustomerBasketDTO>> GetCustomerBasket()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return Unauthorized(new ApiResponse(401));
			}

			var basket = await _basketRepository.GetBasketAsync(user.Id);

			if (basket != null)
			{
				var basketDto = _mapper.Map<CustomerBasketDTO>(basket);
				return Ok(basketDto);
			}
			else
			{
				return Ok(new CustomerBasketDTO(user.Id));
			}
		}
		[HttpPost("AddProduct")]
		public async Task<ActionResult<CustomerBasketDTO>> AddProductToCart(int productId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized(new ApiResponse(401));

			var product = await _unitOfWork.Repository<Product>()
				.GetByIdWithSpecAsync(new ProductSpecs(productId));
			if (product == null)
				return NotFound(new ApiResponse(404, "Product not found"));

			var basket = await _basketRepository.GetBasketAsync(user.Id)
						  ?? new CustomerBasket(user.Id);

			basket.Items ??= new List<BasketItem>();

			var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
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
					ProductId = productId,
					Quantity = 1,
					Price = product.Price,
					Name = product.Name,
					PictureUrl = product.PictureUrl,
					ActiveIngredient = product.ActiveIngredient,
					BasketId = basket.Id
				});
			}

			var updated = await _basketRepository.UpdateBasketAsync(basket);
			if (updated == null)
				return BadRequest(new ApiResponse(400, "Failed to update basket"));

			var dto = _mapper.Map<CustomerBasketDTO>(updated);
			return Ok(dto);
		}

		[HttpDelete("RemoveProduct")]
		public async Task<ActionResult<CustomerBasketDTO>> RemoveProductFromCart(int productId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized(new ApiResponse(401));

			var basket = await _basketRepository.GetBasketAsync(user.Id);
			if (basket == null) return NotFound(new ApiResponse(404, "Basket not found"));

			var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
			if (item == null) return NotFound(new ApiResponse(404, "Product not found in basket"));

			basket.Items.Remove(item);

			var updated = await _basketRepository.UpdateBasketAsync(basket);
			if (updated == null)
				return BadRequest(new ApiResponse(400, "Failed to update basket"));

			var dto = _mapper.Map<CustomerBasketDTO>(updated);
			return Ok(dto);
		}

		[HttpPost]
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDTO basketDto)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized(new ApiResponse(401));

			var mappedBasket = _mapper.Map<CustomerBasketDTO, CustomerBasket>(basketDto);
			mappedBasket.Id = user.Id; // Force basket to be associated with the logged-in user
			
			// Set BasketId for all items
			foreach (var item in mappedBasket.Items)
			{
				item.BasketId = mappedBasket.Id;
			}

			var updated = await _basketRepository.UpdateBasketAsync(mappedBasket);
			if (updated == null)
				return BadRequest(new ApiResponse(400, "Failed to update basket"));

			return Ok(updated);
		}

		[HttpDelete]
		public async Task<ActionResult<bool>> DeleteBasket()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized(new ApiResponse(401));

			return await _basketRepository.DeleteBasketAsync(user.Id);
		}
	}
}
