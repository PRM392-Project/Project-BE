namespace SnapRoom.Contract.Repositories.Dtos.OrderDtos
{
	public class CartItemDto
	{
		public int Quantity { get; set; }
		public string ProductId { get; set; } = default!;
	}
}
