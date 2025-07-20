using SnapRoom.Common.Enum;
using SnapRoom.Common.Utils;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class TrackingStatus
	{
		public DateTimeOffset Time { get; set; } = CoreHelper.SystemTimeNow;

		public StatusEnum StatusId { get; set; } = default!;
		public virtual Status Status { get; set; } = default!;
		public string OrderId { get; set; } = default!;
		public virtual Order Order { get; set; } = default!;
	}
}
