using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Dtos.OrderDtos;

namespace SnapRoom.Contract.Services
{
	public interface IOrderService
	{
		Task<BasePaginatedList<object>> GetOrders(string? customerId, string? designerId, int pageNumber, int pageSize);
		Task<BasePaginatedList<object>> GetOrdersForCustomer(int pageNumber, int pageSize);
		Task<BasePaginatedList<object>> GetOrdersForDesigner(int pageNumber, int pageSize);
		Task<object> GetOrderById(string id);
		Task<object> GetCart();
		Task AddToCart(CartItemDto dto);
		Task UpdateCart(List<CartItemDto> dtos);
		Task DeleteFromCart(string productId);
		Task UpdateCartInfo(CartUpdateDto dto);
		Task ProcessOrder(string orderId, StatusEnum status);
	}
}
