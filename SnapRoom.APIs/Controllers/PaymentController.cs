using Microsoft.AspNetCore.Mvc;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Services;

namespace SnapRoom.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;
		public PaymentController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		[HttpGet]
		public async Task<IActionResult> PayCart()
		{
			var result = await _paymentService.PayCart();

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Chuyển hướng thanh toán",
				data: result
			));
		}

	}
}
