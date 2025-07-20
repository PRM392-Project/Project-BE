#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SnapRoom.Contract.Repositories.Entities;

namespace SnapRoom.Repositories.DatabaseContext
{
	public partial class SnapRoomDbContext : DbContext
	{
		public SnapRoomDbContext(DbContextOptions<SnapRoomDbContext> options) : base(options)
		{
		}

		public virtual DbSet<Account> Accounts { get; set; }
		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Conversation> Conversations { get; set; }
		public virtual DbSet<Design> Designs { get; set; }
		public virtual DbSet<Furniture> Furnitures { get; set; }
		public virtual DbSet<Message> Messages { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<OrderDetail> OrderDetails { get; set; }
		public virtual DbSet<Plan> Plans { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<ProductReview> ProductReviews { get; set; }
		public virtual DbSet<Status> Statuses { get; set; }
		public virtual DbSet<TrackingStatus> TrackingStatuses { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<OrderDetail>()
				.HasKey(o => new { o.OrderId, o.ProductId });
			modelBuilder.Entity<ProductReview>()
				.HasKey(p => new { p.ProductId, p.CustomerId });
			modelBuilder.Entity<TrackingStatus>()
				.HasKey(s => new { s.StatusId, s.OrderId });
			modelBuilder.Entity<ProductCategory>()
				.HasKey(c => new { c.ProductId, c.CategoryId });



			modelBuilder.Entity<Account>()
				.HasMany(a => a.CustomerConversations).WithOne(c => c.Customer).HasForeignKey(c => c.CustomerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasMany(a => a.DesignerConversations).WithOne(c => c.Designer).HasForeignKey(c => c.DesignerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasMany(a => a.Messages).WithOne(m => m.Sender).HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasMany(a => a.CustomerOrders).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasMany(a => a.DesignerOrders).WithOne(o => o.Designer).HasForeignKey(o => o.DesignerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasMany(a => a.ProductReviews).WithOne(p => p.Customer).HasForeignKey(p => p.CustomerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasMany(a => a.Products).WithOne(p => p.Designer).HasForeignKey(p => p.DesignerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasOne(a => a.Avatar).WithOne(i => i.Account).HasForeignKey<Account>(a => a.AvatarSource).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasOne(a => a.Plan).WithMany().HasForeignKey(a => a.PlanId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Account>()
				.HasMany(a => a.Notifications).WithOne(n => n.Account).HasForeignKey(n => n.AccountId).OnDelete(DeleteBehavior.NoAction);


			modelBuilder.Entity<Product>()
				.HasMany(p => p.Furnitures).WithOne(f => f.ParentDesign).HasForeignKey(f => f.ParentDesignId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Product>()
				.HasMany(p => p.ProductReviews).WithOne(p => p.Product).HasForeignKey(p => p.ProductId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Product>()
				.HasMany(p => p.OrderDetails).WithOne(o => o.Product).HasForeignKey(o => o.ProductId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Product>()
				.HasMany(p => p.Images).WithOne(i => i.Product).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Product>()
				.HasMany(p => p.ProductCategories).WithOne(c => c.Product).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Product>()
				.HasOne(p => p.Design).WithOne(d => d.Product).HasForeignKey<Design>(d => d.Id).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Product>()
				.HasOne(p => p.Furniture).WithOne(f => f.Product).HasForeignKey<Furniture>(f => f.Id).OnDelete(DeleteBehavior.NoAction);


			modelBuilder.Entity<Order>()
				.HasMany(o => o.TrackingStatuses).WithOne(t => t.Order).HasForeignKey(t => t.OrderId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Order>()
				.HasMany(o => o.OrderDetails).WithOne(o => o.Order).HasForeignKey(o => o.OrderId).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Conversation>()
				.HasMany(c => c.Messages).WithOne(m => m.Conversation).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<TrackingStatus>()
				.HasOne(c => c.Status).WithMany().HasForeignKey(t => t.StatusId).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<ProductCategory>()
				.HasOne(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.NoAction);
		}
	}
}
