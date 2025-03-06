using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.DTOs;
using PharmAssist.Errors;
using PharmAssist.Extensions;
using PharmAssist.Services;
using System.Security.Claims;


namespace PharmAssist.Controllers
{
	public class AccountsController : APIBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountsController(UserManager<AppUser> userManager,
			   SignInManager<AppUser> signInManager,
			   ITokenService tokenService,
			   IMapper mapper)
        { 
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
			_mapper = mapper;
		}

        
        [HttpPost("Register")]
		public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
		{
			if (CheckEmailExists(model.Email).Result.Value)
				return BadRequest(new ApiResponse(400, "This email is already in use"));

			var user = new AppUser()
			{
				DisplayName=model.DisplayName,
				Email=model.Email,
				UserName = model.Email.Split('@')[0],
				PhoneNumber = model.PhoneNumber,
			};
			var result= await _userManager.CreateAsync(user,model.Password); 

			if (!result.Succeeded) return BadRequest(new ApiResponse(400));

			var ReturnedUser = new UserDTO()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token =await  _tokenService.CreateTokenAsync(user, _userManager)
			};
			return Ok(ReturnedUser);
		}


		
		[HttpPost("Login")]
		public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
		{
			var user=await _userManager.FindByEmailAsync(model.Email);
			if (user is null) return Unauthorized(new ApiResponse(401));

			var result= await _signInManager.CheckPasswordSignInAsync(user, model.Password,false);
			if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

			return Ok(new UserDTO()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			});

		}


		[Authorize]
		[HttpGet("GetCurrentUser")]
		public async Task<ActionResult<UserDTO>> GetCurrentUser()
		{
			var email= User.FindFirstValue(ClaimTypes.Email);
			var user=await _userManager.FindByEmailAsync(email);
			var returnedUser = new UserDTO()
			{
				DisplayName=user.DisplayName,
				Email=user.Email,
				Token=await _tokenService.CreateTokenAsync(user,_userManager)
			};
			return Ok(returnedUser);
		}


		[Authorize]
		[HttpGet("CurrentUserAddress")]
		public async Task<ActionResult<AddressDTO>> GetCurrentAddress()
		{
			var user=await _userManager.FindUserWithAddressAsync(User);
			var mappedAddress=_mapper.Map<Address,AddressDTO>(user.Address);
			return Ok(mappedAddress);
		}


		[Authorize]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDTO>> UpdateAddress(AddressDTO updatedAddress)
		{
			var user = await _userManager.FindUserWithAddressAsync(User);
			if (user is null) return Unauthorized(new ApiResponse(401));
			var address = _mapper.Map<AddressDTO, Address>(updatedAddress);
			address.Id=user.Address.Id; 
			user.Address = address;
			var result= await _userManager.UpdateAsync(user);
			if (!result.Succeeded) return BadRequest(new ApiResponse(400));
			return Ok(updatedAddress);
		}


		[HttpGet("emailExists")]
		 public async Task<ActionResult<bool>> CheckEmailExists(string email)
		{
			return await _userManager.FindByEmailAsync(email) is not null;
		}
	}
}
