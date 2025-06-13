using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Core.Entities.Order_Aggregation;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Services;
using PharmAssist.DTOs;
using PharmAssist.Errors;
using System.Security.Claims;

namespace PharmAssist.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : APIBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Dashboard Statistics

        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<AdminDashboardStatsDTO>> GetDashboardStats()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalProducts = await _unitOfWork.Repository<Product>().CountAsync(null);
            var totalOrders = await _unitOfWork.Repository<Order>().CountAsync(null);
            
            var pendingOrders = await _unitOfWork.Repository<Order>()
                .CountAsync(o => o.Status == OrderStatus.Pending);
            
            var completedOrders = await _unitOfWork.Repository<Order>()
                .CountAsync(o => o.Status == OrderStatus.PaymentReceived);
            
            var deliveredOrders = await _unitOfWork.Repository<Order>()
                .CountAsync(o => o.Status == OrderStatus.Delivered);

            // Calculate total revenue from completed and delivered orders
            var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
            var totalRevenue = orders
                .Where(o => o.Status == OrderStatus.PaymentReceived || o.Status == OrderStatus.Delivered)
                .Sum(o => o.GetTotal);

            // Get recent orders
            var recentOrders = await _unitOfWork.Repository<Order>()
                .GetAsync(o => true, null, "Items,DeliveryMethod", 5, 0);

            var recentOrdersDto = _mapper.Map<IEnumerable<AdminOrderSummaryDTO>>(recentOrders);

            return Ok(new AdminDashboardStatsDTO
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                DeliveredOrders = deliveredOrders,
                TotalRevenue = totalRevenue,
                RecentOrders = recentOrdersDto
            });
        }

        #endregion

        #region User Management

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AdminUserDTO>>> GetAllUsers(
            [FromQuery] int pageIndex = 0, 
            [FromQuery] int pageSize = 10)
        {
            var users = await _userManager.Users
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = new List<AdminUserDTO>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<AdminUserDTO>(user);
                userDto.Roles = roles.ToList();
                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<AdminUserDetailDTO>> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new ApiResponse(404, "User not found"));

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<AdminUserDetailDTO>(user);
            userDto.Roles = roles.ToList();

            // Get user's orders
            var userOrders = await _unitOfWork.Repository<Order>()
                .GetAsync(o => o.BuyerEmail == user.Email, null, "Items,DeliveryMethod");
            userDto.Orders = _mapper.Map<IEnumerable<AdminOrderSummaryDTO>>(userOrders);

            return Ok(userDto);
        }

        [HttpPut("users/{id}/roles")]
        public async Task<ActionResult> UpdateUserRoles(string id, [FromBody] UpdateUserRolesDTO dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new ApiResponse(404, "User not found"));

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, dto.Roles);

            return Ok(new { success = true, message = "User roles updated successfully" });
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new ApiResponse(404, "User not found"));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Failed to delete user"));

            return Ok(new { success = true, message = "User deleted successfully" });
        }

        #endregion

        #region Product Management

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<AdminProductDTO>>> GetAllProducts(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 10)
        {
            var products = await _unitOfWork.Repository<Product>()
                .GetAsync(null, null, "", pageSize, pageIndex);

            var productDtos = _mapper.Map<IEnumerable<AdminProductDTO>>(products);
            return Ok(productDtos);
        }

        [HttpPost("products")]
        public async Task<ActionResult<AdminProductDTO>> CreateProduct([FromBody] CreateProductDTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            _unitOfWork.Repository<Product>().Add(product);
            
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to create product"));

            var productDto = _mapper.Map<AdminProductDTO>(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, productDto);
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult<AdminProductDTO>> GetProductById(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            var productDto = _mapper.Map<AdminProductDTO>(product);
            return Ok(productDto);
        }

        [HttpPut("products/{id}")]
        public async Task<ActionResult<AdminProductDTO>> UpdateProduct(int id, [FromBody] UpdateProductDTO dto)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            _mapper.Map(dto, product);
            _unitOfWork.Repository<Product>().Update(product);
            
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to update product"));

            var productDto = _mapper.Map<AdminProductDTO>(product);
            return Ok(productDto);
        }

        [HttpDelete("products/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            _unitOfWork.Repository<Product>().Delete(product);
            
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to delete product"));

            return Ok(new { success = true, message = "Product deleted successfully" });
        }

        #endregion

        #region Order Management

        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<AdminOrderDTO>>> GetAllOrders(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderStatus? status = null)
        {
            var orders = await _unitOfWork.Repository<Order>()
                .GetAsync(
                    o => status == null || o.Status == status,
                    o => o.OrderByDescending(x => x.OrderDate),
                    "Items,DeliveryMethod",
                    pageSize,
                    pageIndex);

            var orderDtos = _mapper.Map<IEnumerable<AdminOrderDTO>>(orders);
            return Ok(orderDtos);
        }

        [HttpGet("orders/{id}")]
        public async Task<ActionResult<AdminOrderDetailDTO>> GetOrderById(int id)
        {
            var order = await _unitOfWork.Repository<Order>()
                .GetEntityWithSpecAsync(o => o.Id == id, "Items,DeliveryMethod");

            if (order == null)
                return NotFound(new ApiResponse(404, "Order not found"));

            var orderDto = _mapper.Map<AdminOrderDetailDTO>(order);
            return Ok(orderDto);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
            if (order == null)
                return NotFound(new ApiResponse(404, "Order not found"));

            order.Status = dto.Status;
            _unitOfWork.Repository<Order>().Update(order);
            
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to update order status"));

            return Ok(new { success = true, message = "Order status updated successfully" });
        }

        #endregion

        #region Configuration Diagnostics

        [HttpGet("config-test")]
        public IActionResult TestConfiguration()
        {
            return Ok(new { 
                ApiBaseUrl = _configuration["ApiBaseUrl"],
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Not Set",
                CurrentTime = DateTime.UtcNow
            });
        }

        #endregion

        #region System Settings

        [HttpGet("system-info")]
        public async Task<ActionResult<SystemInfoDTO>> GetSystemInfo()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalProducts = await _unitOfWork.Repository<Product>().CountAsync(null);
            var totalOrders = await _unitOfWork.Repository<Order>().CountAsync(null);

            return Ok(new SystemInfoDTO
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                SystemVersion = "1.0.0",
                LastBackupDate = DateTime.UtcNow.AddDays(-1), // Mock data
                DatabaseStatus = "Healthy"
            });
        }

        #endregion
    }
} 