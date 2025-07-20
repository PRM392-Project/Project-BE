using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Account : BaseEntity
	{
		public string Name { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string Password { get; set; } = default!;
		public string? Profession { get; set; }
		public string? ContactNumber { get; set; }
		public RoleEnum Role { get; set; }
		public string? ApplicationUrl { get; set; }
		public string? VerificationToken { get; set; }
		public string? ResetPasswordToken { get; set; }

		public string? PlanId { get; set; }
		public virtual Plan? Plan { get; set; }
		public string? AvatarSource { get; set; }
		public virtual Image? Avatar { get; set; }

		public virtual ICollection<Conversation>? CustomerConversations { get; set; }
		public virtual ICollection<Conversation>? DesignerConversations { get; set; }

		public virtual ICollection<Message>? Messages { get; set; }
		public virtual ICollection<Order>? CustomerOrders { get; set; }
		public virtual ICollection<Order>? DesignerOrders { get; set; }
		public virtual ICollection<ProductReview>? ProductReviews { get; set; }
		public virtual ICollection<Product>? Products { get; set; }
		public virtual ICollection<Notification>? Notifications { get; set; }
	}
}
