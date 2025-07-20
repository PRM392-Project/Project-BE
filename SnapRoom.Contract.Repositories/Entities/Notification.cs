using SnapRoom.Common.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Notification
	{
		[Key]
		[Column(TypeName = "nvarchar(36)")]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		public string Title { get; set; } = default!;

		public string? Type { get; set; }
		public string? TypeId { get; set; }

		public bool IsRead { get; set; }
		public DateTimeOffset CreatedAt { get; set; } = CoreHelper.SystemTimeNow;
		public DateTimeOffset? ReadAt { get; set; }

		public string? AccountId { get; set; }
		public  virtual Account? Account { get; set; }
	}
}
