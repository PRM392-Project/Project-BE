using SnapRoom.Common.Base;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Product : BaseEntity
	{
		public string Name { get; set; } = default!;
		public decimal Price { get; set; }
		public double Rating { get; set; } = 0;
		public string Description { get; set; } = default!;
		public bool Active { get; set; } = false;
		public bool Approved { get; set; } = false;
		public string? DesignerId { get; set; }
		public virtual Account? Designer { get; set; }
		public string? ParentDesignId { get; set; }
		public virtual Product? ParentDesign { get; set; }
		public int? InDesignQuantity { get; set; }
		public virtual ICollection<Product>? Furnitures { get; set; }
		public virtual ICollection<ProductCategory>? ProductCategories { get; set; }
		public virtual ICollection<ProductReview>? ProductReviews { get; set; }
		public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
		public virtual ICollection<Image>? Images { get; set; }
		public virtual Design? Design { get; set; }
		public virtual Furniture? Furniture { get; set; }

	}
}
