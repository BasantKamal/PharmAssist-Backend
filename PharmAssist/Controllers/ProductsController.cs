using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.APIs.DTOs;
using PharmAssist.APIs.Helpers;
using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Specifications;
using PharmAssist.Errors;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PharmAssist.Controllers
{
	public class ProductsController : APIBaseController
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ProductsController> _logger;

		public ProductsController(IMapper mapper,
			                      IUnitOfWork unitOfWork,
			                      ILogger<ProductsController> logger)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		// Remove CachedAttribute to avoid Redis dependency issues
		// [CachedAttribute(300)]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery]ProductSpecParam Params)
		{
			try
			{
				var Spec = new ProductSpecs(Params);
				var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
				var mappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);

				var countSpec = new ProductWithFilterationForCountAsync(Params);
				var count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);

				return Ok(new Pagination<ProductToReturnDTO>(Params.PageIndex, Params.PageSize, mappedProducts, count));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving products");
				return StatusCode(500, new ApiResponse(500, "An error occurred while retrieving products"));
			}
		}

		[HttpGet("search")]
		public async Task<ActionResult<IReadOnlyList<ProductToReturnDTO>>> SearchProducts([FromQuery] SearchProductsParam searchParams)
		{
			try
			{
				var spec = new ProductSearchSpecs(searchParams);
				var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
				var mappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);

				// Return all products directly without pagination
				return Ok(mappedProducts);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error searching products");
				return StatusCode(500, new ApiResponse(500, "An error occurred while searching products"));
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProductById(int id)
		{
			try
			{
				var Spec = new ProductSpecs(id);
				var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);
				if (product is null)
					return NotFound(new ApiResponse(404));
				var mappedProducts = _mapper.Map<Product, ProductToReturnDTO>(product);
				return Ok(mappedProducts);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
				return StatusCode(500, new ApiResponse(500, "An error occurred while retrieving the product"));
			}
		}
	}
}
