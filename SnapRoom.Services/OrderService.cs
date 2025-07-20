using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Common.Utils;
using SnapRoom.Contract.Repositories.Dtos.OrderDtos;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;
using static SnapRoom.Common.Base.BaseException;

namespace SnapRoom.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IAuthService _authService;

		public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_authService = authService;
		}

		public async Task<BasePaginatedList<object>> GetOrders(string? customerId, string? designerId, int pageNumber, int pageSize)
		{
			List<Order> query = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => (string.IsNullOrWhiteSpace(customerId) || o.CustomerId == customerId) && (string.IsNullOrWhiteSpace(designerId) || o.DesignerId == designerId) && !o.IsCart).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.OrderPrice,
				Customer = new { x.Customer?.Name },
				Designer = new { x.Designer?.Name },
				x.Address,
				x.PhoneNumber,
				x.Method,
				Date = _unitOfWork.GetRepository<TrackingStatus>().Entities.FirstOrDefault(t => t.OrderId == x.Id && t.StatusId == StatusEnum.Pending)?.Time,
				Status = _unitOfWork.GetRepository<TrackingStatus>().Entities.Where(t => t.OrderId == x.Id).OrderByDescending(t => t.Time).FirstOrDefault()?.Status.Name,
			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}

		public async Task<BasePaginatedList<object>> GetOrdersForCustomer(int pageNumber, int pageSize)
		{
			string customerId = _authService.GetCurrentAccountId();

			Account? customer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == customerId && a.Role == RoleEnum.Customer).FirstOrDefaultAsync();

			if (customer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không tồn tại");
			}

			return await GetOrders(customerId, null, pageNumber, pageSize);
		}

		public async Task<BasePaginatedList<object>> GetOrdersForDesigner(int pageNumber, int pageSize)
		{
			string designerId = _authService.GetCurrentAccountId();

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer).FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không tồn tại");
			}

			return await GetOrders(null, designerId, pageNumber, pageSize);
		}

		public async Task<object> GetOrderById(string id)
		{
			Order? order = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => o.Id == id && !o.IsCart).FirstOrDefaultAsync();

			if (order == null)
			{
				throw new ErrorException(404, "", "Không tìm thấy đơn hàng");
			}

			var responseItem = new
			{
				order.Id,
				order.OrderPrice,
				Customer = new { order.Customer.Name },
				Designer = new { order.Designer?.Name },
				order.Address,
				order.PhoneNumber,
				order.Method,
				Status = _unitOfWork.GetRepository<TrackingStatus>().Entities.Where(t => t.OrderId == order.Id).OrderByDescending(t => t.Time).FirstOrDefault()?.Status.Name,
				OrderDetails = order.OrderDetails?.Select(od => new
				{
					Product = new
					{
						od.Product.Id,
						od.Product.Name,
						od.Product.Price,
						IsDesign = (od.Product.Design != null) ? true : false,
						PrimaryImage = new { od.Product.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
					},
					od.Quantity,
					od.DetailPrice
				}).ToList(),
				Statuses = order.TrackingStatuses?.Select(ts => new
				{
					ts.Status.Name,
					ts.Time
				}).ToList(),
			};

			return responseItem;
		}

		public async Task<object> GetCart()
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

			if (cart == null)
			{
				throw new ErrorException(400, "", "Vui lòng chọn sản phẩm");
			}

			var responseItem = new
			{
				cart.Id,
				cart.OrderPrice,
				cart.Address,
				cart.PhoneNumber,
				cart.Method,
				OrderDetails = cart.OrderDetails?.Select(od => new
				{
					Product = new
					{
						od.Product.Id,
						od.Product.Name,
						od.Product.Price,
						IsDesign = (od.Product.Design != null) ? true : false,
						PrimaryImage = new { od.Product.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
					},
					od.Quantity,
					od.DetailPrice
				}).ToList(),
			};

			return responseItem;
		}

		public async Task AddToCart(CartItemDto dto)
		{
			string customerId = _authService.GetCurrentAccountId();

			Account? customer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == customerId && a.Role == RoleEnum.Customer).FirstOrDefaultAsync();

			if (customer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			Product? product = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Id == dto.ProductId && p.DeletedBy == null).FirstOrDefaultAsync();

			if (product == null)
			{
				throw new ErrorException(404, "", "Sản phẩm không tồn tại");
			}

			if (dto.Quantity <= 0)
			{
				dto.Quantity = 1;
			}


			Order cart = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => o.CustomerId == customerId && o.IsCart).FirstOrDefaultAsync() ?? new() 
				{
					CustomerId = customerId,
					DesignerId = product.DesignerId,
					Method = MethodEnum.COD,
				};


			// Case old cart & old detail
			if (cart.OrderDetails is not null && cart.OrderDetails.Count > 0)
			{
				foreach (var item in cart.OrderDetails)
				{
					if (item.Product.DesignerId != product.DesignerId)
						throw new ErrorException(404, "", "Mỗi đơn hàng chỉ áp dụng cho sản phẩm của 1 nhà thiết kế");

					if (item.ProductId == dto.ProductId)
					{
						item.Quantity += dto.Quantity;
						item.DetailPrice = item.Quantity * item.Product.Price;
						cart.OrderPrice += item.Product.Price * dto.Quantity;
						_unitOfWork.GetRepository<OrderDetail>().Update(item);
						await _unitOfWork.SaveAsync();
						return;
					}
				}

			}

			if (cart.OrderDetails is null || cart.OrderDetails.Count == 0)
			{
				await _unitOfWork.GetRepository<Order>().InsertAsync(cart);
			}

			OrderDetail orderDetail = new()
			{
				OrderId = cart.Id,
				ProductId = dto.ProductId,
				Quantity = dto.Quantity,
				DetailPrice = dto.Quantity * product.Price,
				Product = product,
			};
			cart.OrderPrice += orderDetail.DetailPrice;
			await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(orderDetail);

			await _unitOfWork.SaveAsync();
		}

		public async Task UpdateCart(List<CartItemDto> dtos)
		{
			string customerId = _authService.GetCurrentAccountId();

			Account? customer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == customerId && a.Role == RoleEnum.Customer).FirstOrDefaultAsync();
			if (customer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			Order? cart = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => o.CustomerId == customerId && o.IsCart).FirstOrDefaultAsync();
			if (cart == null) 
			{
				throw new ErrorException(404, "", "Giỏ hàng hiện tại không tồn tại");
			}

			foreach(var cartItem in dtos)
			{
				//Only furniture needs to be updated in the cart
				Product? product = await _unitOfWork.GetRepository<Product>().Entities
					.Where(p => p.Id == cartItem.ProductId && p.DeletedBy == null).FirstOrDefaultAsync();

				if (product == null)
				{
					throw new ErrorException(404, "", "Sản phẩm không tồn tại");
				}
				if (product.Design != null) 
				{
					continue;
				}

				if (cartItem.Quantity <= 0)
				{
					cartItem.Quantity = 1;
				}

				OrderDetail? detail = cart.OrderDetails?.FirstOrDefault(od => od.ProductId == cartItem.ProductId);
				if (detail == null)
				{
					throw new ErrorException(404, "", $"Sản phẩm không có trong giỏ hàng");
				}
				if (detail.Product.DesignerId != product.DesignerId)
				{
					throw new ErrorException(404, "", "Mỗi đơn hàng chỉ áp dụng cho sản phẩm của 1 nhà thiết kế");
				}
				if (detail.Quantity == cartItem.Quantity)
				{
					continue;
				}
				cart.OrderPrice -= detail.DetailPrice; // Remove old price
				detail.Quantity = cartItem.Quantity;
				detail.DetailPrice = detail.Quantity * detail.Product.Price; // Update price
				cart.OrderPrice += detail.DetailPrice; // Add new price
				_unitOfWork.GetRepository<OrderDetail>().Update(detail);
			}

			_unitOfWork.GetRepository<Order>().Update(cart);

			await _unitOfWork.SaveAsync();
		}

		public async Task DeleteFromCart(string productId)
		{
			string customerId = _authService.GetCurrentAccountId();

			// Fetch cart of the customer
			Order? cart = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => o.CustomerId == customerId && o.IsCart)
				.FirstOrDefaultAsync();

			if (cart == null || cart.OrderDetails == null || cart.OrderDetails.Count == 0)
				throw new ErrorException(404, "", "Giỏ hàng hiện tại không tồn tại");

			// Find the item in the cart
			OrderDetail? detail = cart.OrderDetails.FirstOrDefault(od => od.ProductId == productId);

			if (detail == null)
				throw new ErrorException(404, "", "Sản phẩm không có trong giỏ hàng");

			// Update cart total
			cart.OrderPrice -= detail.DetailPrice;

			// Remove the order detail
			_unitOfWork.GetRepository<OrderDetail>().Delete(detail);

			// Optionally remove cart if it's now empty
			if (cart.OrderDetails.Count == 1) // because we’re deleting 1
			{
				_unitOfWork.GetRepository<Order>().Delete(cart);
			}
			else
			{
				_unitOfWork.GetRepository<Order>().Update(cart);
			}

			await _unitOfWork.SaveAsync();
		}

		public async Task UpdateCartInfo(CartUpdateDto dto)
		{
			string customerId = _authService.GetCurrentAccountId();

			// Fetch cart of the customer
			Order? cart = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => o.CustomerId == customerId && o.IsCart)
				.FirstOrDefaultAsync();

			if (cart == null || cart.OrderDetails == null || cart.OrderDetails.Count == 0)
				throw new ErrorException(404, "", "Giỏ hàng hiện tại không tồn tại");

			cart.Address = dto.Address;
			cart.PhoneNumber = dto.PhoneNumber;
			cart.Method = dto.Method;

			await _unitOfWork.SaveAsync();
		}

		public async Task ProcessOrder(string orderId, StatusEnum status)
		{
			Order? cart = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => o.Id == orderId).FirstOrDefaultAsync();

			if (cart == null)
			{
				throw new ErrorException(404, "", "Giỏ hàng hiện tại không tồn tại");
			}

			cart.IsCart = false;

			TrackingStatus trackingStatus = new()
			{
				OrderId = cart.Id,
				StatusId = status,
				Time = CoreHelper.SystemTimeNow
			};

			await _unitOfWork.GetRepository<TrackingStatus>().InsertAsync(trackingStatus);
			try
			{
				await _unitOfWork.SaveAsync();

			}
			catch (Exception ex) { }
		}
	}
}
