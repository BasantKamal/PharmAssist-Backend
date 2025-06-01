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
