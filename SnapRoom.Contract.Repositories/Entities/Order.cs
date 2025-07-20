using SnapRoom.Common.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Order
	{
		[Key]
		[Column(TypeName = "nvarchar(36)")]
		public string Id { get; set; } = Guid.NewGuid().ToString().ToUpper();
		public decimal OrderPrice { get; set; }
		public MethodEnum Method { get; set; }
		public bool IsCart { get; set; } = true;
		public string? Address { get; set; }

		public string CustomerId { get; set; } = default!;
		public virtual Account Customer { get; set; } = default!;
		public string? DesignerId { get; set; }
		public virtual Account? Designer { get; set; }
		public string? PhoneNumber { get; set; }
		public virtual ICollection<TrackingStatus>? TrackingStatuses { get; set; }
		public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
	}
}
