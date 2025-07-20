namespace SnapRoom.Contract.Repositories.Entities
{
	public class ProductCategory
	{
		public string ProductId { get; set; } = default!;
		public virtual Product Product { get; set; } = default!;
		public string CategoryId { get; set; } = default!;
		public virtual Category Category { get; set; } = default!;
	}
}
