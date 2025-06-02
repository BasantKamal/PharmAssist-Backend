using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.APIs.DTOs;
using PharmAssist.Controllers;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using PharmAssist.Errors;


namespace PharmAssist.APIs.Controllers
{
	public class BasketsController : APIBaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketsController(IBasketRepository basketRepository,IMapper mapper)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
		}	

		//Test Redis Connection
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
