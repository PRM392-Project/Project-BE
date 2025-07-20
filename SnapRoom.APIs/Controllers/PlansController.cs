using Microsoft.AspNetCore.Mvc;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Services;
using SnapRoom.Services;

namespace SnapRoom.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlansController : ControllerBase
	{
		private readonly IPlanService _planService;

		public PlansController(IPlanService planService)
		{
			_planService = planService;
		}

		[HttpPost]
		public async Task<IActionResult> CreatePlan()
		{
			await _planService.CreatePlan();

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Tạo gói thành công",
				data: null
			));
		}

	}
}
