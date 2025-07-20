namespace SnapRoom.Contract.Repositories.Entities
{
	public class OrderDetail
	{
		public int Quantity { get; set; }
		public decimal DetailPrice { get; set; }

		public string OrderId { get; set; } = default!;
		public virtual Order Order { get; set; } = default!;
		public string ProductId { get; set; } = default!;
		public virtual Product Product { get; set; } = default!;
	}
}
