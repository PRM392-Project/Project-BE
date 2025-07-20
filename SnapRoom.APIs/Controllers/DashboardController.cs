using Microsoft.AspNetCore.Mvc;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Services;

namespace SnapRoom.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DashboardController : ControllerBase
	{
		private readonly IDashboardService _dashboardService;

		public DashboardController(IDashboardService dashboardService)
		{
			_dashboardService = dashboardService;
		}

		[HttpGet("revenue-by-day")]
		public async Task<IActionResult> GetRevenueByDay(int month, int year)
		{
			var result = await _dashboardService.GetRevenueByDay(month, year);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

		[HttpGet("top-designers-by-revenue")]
		public async Task<IActionResult> GetTopDesignersByRevenue(int topN)
		{
			var result = await _dashboardService.GetTopDesignersByRevenue(topN);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

		[HttpGet("orders")]
		public async Task<IActionResult> GetOrders()
		{
			var result = await _dashboardService.GetOrders();
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

		[HttpGet("user-growth")]
		public async Task<IActionResult> GetMonthlyUserGrowth()
		{
			var result = await _dashboardService.GetMonthlyUserGrowth();
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

		[HttpGet("designer/revenue-by-day")]
		public async Task<IActionResult> GetRevenueByDayForDesigner(int month, int year)
		{
			var result = await _dashboardService.GetRevenueByDayForDesigner(month, year);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

		[HttpGet("top-products")]
		public async Task<IActionResult> GetTopSellingProducts()
		{
			var result = await _dashboardService.GetTopSellingProducts(5);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

		[HttpGet("top-products-reviews")]
		public async Task<IActionResult> GetTopSellingProductsWithComments()
		{
			var result = await _dashboardService.GetTopSellingProductsWithComments(3);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

		[HttpGet("total-reviews")]
		public async Task<IActionResult> GetTotalProductReviews()
		{
			var result = await _dashboardService.GetTotalProductReviews();
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy dữ liệu thành công",
				data: result
			));
		}

	}
}
