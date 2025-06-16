using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Core.Services;
using PharmAssist.DTOs;
using PharmAssist.Errors;
using PharmAssist.Extensions;
using PharmAssist.Repository.Services;
using System.Security.Claims;


namespace PharmAssist.Controllers
{
	public class AccountsController : APIBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;
        private readonly OtpService _otpService;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IMapper mapper,
            IEmailService emailService,
            OtpService otpService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _emailService = emailService;
            _otpService = otpService;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiResponse(400, "This email is already in use"));

			var user = new AppUser()
			{
				DisplayName = model.Name,
				Email = model.Email,
				UserName = model.Email.Split('@')[0],
				EmailConfirmed = false
			};
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            await _otpService.SendOtpAsync(user.Email); // <--- sends + stores OTP
            
            return Ok(new
			{
				success= true,
				Message= "User registered successfully. Please check your email for the OTP.",
			});
        }


        [HttpPost("Login")]
		public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user is null) return Unauthorized(new ApiResponse(401));

			if(!user.EmailConfirmed)
			{
				return BadRequest(new ApiResponse(400, "Email not confirmed. Please check your email for the OTP."));
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
			if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

			return Ok(new UserDTO()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			});
		}

		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null) return BadRequest(new ApiResponse(400, "User not found."));

			// Generate and send OTP
			await _otpService.SendOtpAsync(user.Email);

			return Ok(new { success = true, message = "OTP sent to your email for password reset." });
		}

		[HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { success = false, message = "Invalid data." });

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return BadRequest(new { success = false, message = "User not found." });

			var resetResult = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password);
			if (!resetResult.Succeeded)
			{
				var errors = resetResult.Errors.Select(e => e.Description);
				return BadRequest(new { success = false, message = "Password reset failed.", errors });
			}

			return Ok(new { success = true, message = "Password has been reset." });
		}

		[Authorize]
		[HttpPost("EditProfile")]
		public async Task<IActionResult> EditProfile([FromBody] EditProfileDto dto)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized(new ApiResponse(401));

			user.DisplayName = dto.DisplayName;
			user.PhoneNumber = dto.PhoneNumber;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
				return BadRequest(new { success = false, errors = result.Errors.Select(e => e.Description) });

			return Ok(new EditProfileDto()
			{
				DisplayName = user.DisplayName,
				PhoneNumber = dto.PhoneNumber,
			});
		}

		[Authorize]
		[HttpGet("GetCurrentUser")]
		public async Task<ActionResult<UserDTO>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(email);
			var returnedUser = new UserDTO()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			};
			return Ok(returnedUser);
		}

		#region Questions	
		[Authorize]
		[HttpGet("GetAnswers")]
		public async Task<ActionResult<UserProfileDto>> GetMyProfile()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null) return Unauthorized();

			var dto = _mapper.Map<UserProfileDto>(user);

			return Ok(dto);
		}

		[Authorize]
		[HttpPost("UpdateAnswers")]
		public async Task<IActionResult> SaveMyProfile([FromBody] UserProfileDto dto)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();

			_mapper.Map(dto, user);

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
				return BadRequest(new { success = false, errors = result.Errors.Select(e => e.Description) });

			return Ok(new { success = true , dto });
		}
		#endregion

		[Authorize]
		[HttpGet("CurrentUserAddress")]
		public async Task<ActionResult<AddressDTO>> GetCurrentUserAddress()
		{
			var user = await _userManager.FindUserWithAddressAsync(User);
			if (user?.Address == null) 
			{
				return NotFound(new ApiResponse(404, "User address not found"));
			}
			var mappedAddress = _mapper.Map<Address, AddressDTO>(user.Address);
			return Ok(mappedAddress);
		}


		[Authorize]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDTO>> UpdateAddress(AddressDTO updatedAddress)
		{
			var user = await _userManager.FindUserWithAddressAsync(User);
			if (user is null) return Unauthorized(new ApiResponse(401));
			
			var address = _mapper.Map<AddressDTO, Address>(updatedAddress);
			
			// Check if user already has an address
			if (user.Address != null)
			{
				// Update existing address - preserve the ID
				address.Id = user.Address.Id;
			}
			else
			{
				// Creating new address - set the foreign key
				address.AppUserId = user.Id;
			}
			
			user.Address = address;
			var result = await _userManager.UpdateAsync(user);
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
