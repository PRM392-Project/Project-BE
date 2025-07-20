using SnapRoom.Common.Base;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Message : BaseEntity
	{
		public string Content { get; set; } = default!;

		public string ConversationId { get; set; } = default!;
		public virtual Conversation Conversation { get; set; } = default!;
		public string SenderId { get; set; } = default!;
		public virtual Account Sender { get; set; } = default!;
	}
}
