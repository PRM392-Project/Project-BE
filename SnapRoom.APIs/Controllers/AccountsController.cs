using Microsoft.AspNetCore.Mvc;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Dtos.AccountDtos;
using SnapRoom.Contract.Services;
using SnapRoom.Services;

namespace SnapRoom.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly IAccountService _accountService;

		public AccountsController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAccounts(RoleEnum? role, int pageNumber = -1, int pageSize = -1)
		{
			var accounts = await _accountService.GetAccounts(role, pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy tài khoản thành công",
				data: accounts
			));
		}

		[HttpGet("awaiting-designers")]
		public async Task<IActionResult> GetAwaitingDesigners(int pageNumber = -1, int pageSize = -1)
		{
			var accounts = await _accountService.GetAwaitingDesigners(pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy tài khoản thành công",
				data: accounts
			));
		}

	}
}
