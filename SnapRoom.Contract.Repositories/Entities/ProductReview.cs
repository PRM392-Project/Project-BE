using SnapRoom.Common.Base;
using SnapRoom.Common.Utils;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class ProductReview
	{
		public string Comment { get; set; } = default!;
		public int Star {  get; set; }
		public DateTimeOffset Time { get; set; } = CoreHelper.SystemTimeNow;

		public string? CustomerId { get; set; }
		public virtual Account? Customer { get; set; }
		public string? ProductId { get; set; }
		public virtual Product? Product { get; set; }
	}
}
