using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Repository.Services;



[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly OtpService _otpService;
	private readonly UserManager<AppUser> _userManager;
	private readonly ITokenService _tokenService;



	public OtpController(OtpService otpService,
		                 UserManager<AppUser> userManager, 
						 ITokenService tokenService)
    {
        _otpService = otpService;
		_userManager = userManager;
		_tokenService = tokenService;
    }

    [HttpPost("Send")]
    public async Task<IActionResult> SendOtp([FromBody] string email)
    {
        await _otpService.SendOtpAsync(email);
        return Ok("Otp sent");
    }


	[HttpPost("VerifyOtp")]
	public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
	{
		var (isValid, errorMessage) = await _otpService.VerifyOtpAsync(request.Email, request.Code);
		if (!isValid)
			return BadRequest(new { success = false, message = errorMessage });

		try
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return BadRequest(new { success = false, message = "User not found." });

			user.EmailConfirmed = true;
			var updateResult = await _userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
				return BadRequest(new { success = false, message = "User update failed" });

			var token = await _tokenService.CreateTokenAsync(user, _userManager);

			return Ok(new { message = "OTP verified, app access granted.", success = true, token });
		}
		catch (Exception ex)
		{
			return BadRequest(new { success = false, message = "An error occurred while verifying the OTP." });
		}

	}

	[HttpPost("VerifyResetOtp")]
	public async Task<IActionResult> VerifyResetOtp([FromBody] OtpVerifyRequest request)
	{
		if (!ModelState.IsValid)
			return BadRequest(new { success = false, message = "Invalid request." });

		var (isValid, err) = await _otpService.VerifyOtpAsync(request.Email, request.Code);
		if (!isValid)
			return BadRequest(new { success = false, message = err });

		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user == null)
			return BadRequest(new { success = false, message = "User not found." });

		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

		return Ok(new
		{
			success = true,
			message = "OTP verified.",
			resetToken 
		});
	}


	[HttpPost("Resend")]
	public async Task<IActionResult> ResendOtp([FromBody] EmailRequest request)
	{
		if (string.IsNullOrEmpty(request.Email))
		{
			return BadRequest(new { success = false, message = "Email is required." });
		}

		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user == null)
		{
			return BadRequest(new { success = false, message = "User not found." });
		}

		if (user.EmailConfirmed)
		{
			return BadRequest(new { success = false, message = "Email is already confirmed." });
		}

		try
		{
			await _otpService.SendOtpAsync(request.Email); 
			return Ok(new { success = true, message = "OTP resent successfully. Please check your email." });
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, 
				message = "An error occurred while resending the OTP." });
		}
	}

}

public class OtpVerifyRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}
public class EmailRequest
{
	public string Email { get; set; }
}
