using Microsoft.AspNetCore.Mvc;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Dtos.AccountDtos;
using SnapRoom.Contract.Services;

namespace SnapRoom.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IConfiguration _config;

		public AuthController(IAuthService authService, IConfiguration config)
		{
			_authService = authService;
			_config = config;
		}

		[HttpPost("customer/login")]
		public async Task<IActionResult> CustomerLogin(LoginDto loginDto)
		{
			string token = await _authService.CustomerLogin(loginDto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Đăng nhập thành công",
				data: token
			));
		}

		[HttpPost("designer/login")]
		public async Task<IActionResult> DesignerLogin(LoginDto loginDto)
		{
			string token = await _authService.DesignerLogin(loginDto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Đăng nhập thành công",
				data: token
			));
		}

		[HttpPost("customer/register")]
		public async Task<IActionResult> CustomerRegister(RegisterDto registerDto)
		{
			await _authService.CustomerRegister(registerDto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Đăng ký thành công",
				data: null
			));
		}

		[HttpPost("designer/register")]
		public async Task<IActionResult> DesignerRegister(DesignerRegisterDto registerDto)
		{
			await _authService.DesignerRegister(registerDto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Đăng ký thành công",
				data: null
			));
		}

		[HttpGet("verify-account")]
		public async Task<IActionResult> VerifyAccount(string token)
		{
			await _authService.VerifyAccount(token);

			return Redirect(_config["FRONTEND_URL"]! + "/login");
		}

		[HttpPost("application-result")]
		public async Task<IActionResult> VerifyAccount(string email, bool isApproved = true)
		{
			await _authService.SendApplicationResultEmail(email, isApproved);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Xét duyệt thành công",
				data: null
			));
		}

		[HttpPost("update-password")]
		public async Task<IActionResult> VerifyAccount(string password, string newPassword)
		{
			await _authService.UpdatePassword(password, newPassword);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Đặt lại mật khẩu thành công",
				data: null
			));
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(string token, string newPassword)
		{
			await _authService.ResetPassword(token, newPassword);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Thay đổi mật khẩu thành công",
				data: null
			));
		}


		[HttpGet("verify-reset-password")]
		public async Task<IActionResult> VerifyResetPassowrd(string token)
		{
			await _authService.VerifyResetPassowrd(token);

			return Redirect(_config["FRONTEND_URL"]! + $"/reset-password?token={token}");
		}

		[HttpPost("forget-password")]
		public async Task<IActionResult> ForgetPassword(RoleEnum role, string email)
		{
			await _authService.ForgetPassword(role, email);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Forget password",
				data: null
			));
		}
	}
}
