using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.APIs.DTOs;
using PharmAssist.Controllers;
using PharmAssist.Core;
using PharmAssist.Core.Entities.Order_Aggregation;
using PharmAssist.Core.Services;
using PharmAssist.DTOs;
using PharmAssist.Errors;
using System.Security.Claims;


namespace PharmAssist.APIs.Controllers
{

	public class OrdersController : APIBaseController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public OrdersController(IOrderService orderService,IMapper mapper,IUnitOfWork unitOfWork)
        {
			_orderService = orderService;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

        //create order
        [HttpPost]
		[Authorize]
		public async Task<ActionResult<Order>> CreateOrder(OrderDTO orderDTO)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var mappedAddress=_mapper.Map<AddressDTO,Address>(orderDTO.ShippingAddress);
			var order =await _orderService.CreateOrderAsync(buyerEmail, orderDTO.BasketId, orderDTO.DeliveryMethodId, mappedAddress);
			if (order is null) return BadRequest(new ApiResponse(400, "There is a problem with your order"));
			return Ok(order);
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> GetOrdersForUser()
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetOrdersForSpecificUserAsync(buyerEmail);
			if (orders is null) return NotFound(new ApiResponse(404, "There is no orders for this user"));
			var mappedOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDTO>>(orders);
			return Ok(mappedOrders);
		}

		[Authorize]
		[HttpGet("{id}")]
		public async Task<ActionResult<OrderToReturnDTO>> GetOrderByIdForUser(int id)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var order=await _orderService.GetOrderByIdForSpecificUserAsync(buyerEmail, id);
			if (order is null) return NotFound(new ApiResponse(404, $"There is no order with Id = {id} for this user"));
			var mappedOrder = _mapper.Map<Order, OrderToReturnDTO>(order);
			return Ok(mappedOrder);
		}

		[HttpGet("DeliveryMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var deliveryMethods=await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
			return Ok(deliveryMethods);
		}


	}
}
