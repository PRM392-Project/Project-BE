using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;
using static SnapRoom.Common.Base.BaseException;

namespace SnapRoom.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _config;
		private readonly IAuthService _authService;
		private readonly string clientId;
		private readonly string apiKey;
		private readonly string checksumKey;

		public PaymentService(IUnitOfWork unitOfWork, IConfiguration config, IAuthService authService)
		{
			_unitOfWork = unitOfWork;
			_config = config;
			_authService = authService;
			clientId = _config["PayOS:CLIENT_ID"] ?? throw new ErrorException(502, "", "Lỗi key PayOS");
			apiKey = _config["PayOS:API_KEY"] ?? throw new ErrorException(502, "", "Lỗi key PayOS");
			checksumKey = _config["PayOS:CHECKSUM_KEY"] ?? throw new ErrorException(502, "", "Lỗi key PayOS");
		}

		public async Task<string> PayCart()
		{
			string customerId = _authService.GetCurrentAccountId();
			Account? customer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == customerId && a.Role == RoleEnum.Customer).FirstOrDefaultAsync();

			if (customer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không tồn tại");
			}

			Order? cart = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => o.CustomerId == customerId && o.IsCart).FirstOrDefaultAsync();

			if (cart == null || cart.OrderDetails == null || !cart.OrderDetails.Any())
			{
				throw new ErrorException(400, "", "Vui lòng chọn sản phẩm");
			}

			List<ItemData> items = new();
			foreach(var item in cart.OrderDetails)
			{
				items.Add(new ItemData(item.Product.Name, item.Quantity, (int)item.Product.Price));
			}

			var payOS = new PayOS(clientId, apiKey, checksumKey);
			string backendUrl = _config["BACKEND_URL"]!;
			string frontendUrl = _config["FRONTEND_URL"]!;

			var paymentLinkRequest = new PaymentData(
				orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
				amount: (int)cart.OrderPrice,
				description: "Thanh toan don hang",
				items: items,
				returnUrl: backendUrl + $"/api/orders/{cart.Id}/1",
				cancelUrl: frontendUrl + $"/customerCart"
			);
			var response = await payOS.createPaymentLink(paymentLinkRequest);

			return response.checkoutUrl;
		}

	}
}
