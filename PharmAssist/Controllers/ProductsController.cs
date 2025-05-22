using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using PharmAssist.APIs.DTOs;
using PharmAssist.APIs.Helpers;
using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Specifications;
using PharmAssist.Errors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PharmAssist.Controllers
{
	public class ProductsController : APIBaseController
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public ProductsController(IMapper mapper,
			                      IUnitOfWork unitOfWork)
		{
			_mapper=mapper;
			_unitOfWork = unitOfWork;
			
		}

		[CachedAttribute(300)]
		//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery]ProductSpecParam Params)
		{
			var Spec = new ProductSpecs(Params);
			var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
			var mappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);

			var countSpec=new ProductWithFilterationForCountAsync(Params);
			var count= await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);

			return Ok(new Pagination<ProductToReturnDTO>(Params.PageIndex, Params.PageSize,mappedProducts,count)); 

		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProductById(int id)
		{
			var Spec = new ProductSpecs(id);
			var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);
			if (product is null)
				return NotFound(new ApiResponse(404));
			var mappedProducts = _mapper.Map<Product, ProductToReturnDTO>(product);
			return Ok(mappedProducts);
		}
	}
}
