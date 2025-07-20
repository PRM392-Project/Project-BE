using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Conversation
	{
		[Key]
		[Column(TypeName = "nvarchar(36)")]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		public string? CustomerId { get; set; }
		public virtual Account? Customer { get; set; }
		public string? DesignerId { get; set; }
		public virtual Account? Designer { get; set; }

		public virtual ICollection<Message>? Messages { get; set; }
	}
}
