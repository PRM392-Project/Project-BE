using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Cms;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;
using System.Runtime.CompilerServices;
using static SnapRoom.Common.Base.BaseException;

namespace SnapRoom.Services
{
	public class DashboardService : IDashboardService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuthService _authService;

		public DashboardService(IUnitOfWork unitOfWork, IAuthService authService)
		{
			_unitOfWork = unitOfWork;
			_authService = authService;
		}

		public async Task<object> GetRevenueByDay(int month, int year)
		{
			var currentYear = DateTime.Now.Year;

			List<Order> orders = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => !o.IsCart && o.TrackingStatuses != null && o.TrackingStatuses.Any(ts => ts.StatusId == StatusEnum.Pending && ts.Time.Month == month && ts.Time.Year == year)).ToListAsync();

			int daysInMonth = DateTime.DaysInMonth(currentYear, month);


			var result = Enumerable.Range(1, daysInMonth)
				.Select(day => new
				{
					day,
					revenue = orders
						.Where(x =>
							x.TrackingStatuses?.FirstOrDefault(ts => ts.StatusId == StatusEnum.Pending)?.Time.Day == day)
						.Sum(x => x.OrderPrice)
				})
				.ToList();

			return result;
		}

		public async Task<object> GetTopDesignersByRevenue(int topN)
		{
			int month = DateTime.Now.Month;
			int year = DateTime.Now.Year;

			// 1. Get all valid orders (filtered by status and date)
			var orders = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => !o.IsCart &&
							o.TrackingStatuses != null &&
							o.TrackingStatuses.Any(ts => ts.StatusId == StatusEnum.Pending) &&
							o.TrackingStatuses.Any(ts => ts.Time.Month == month && ts.Time.Year == year))
				.ToListAsync();

			// 2. Group by Account and calculate total income
			var result = orders
				.GroupBy(o => o.Designer!.Id)
				.Select(g => new
				{
					accountId = g.Key,
					accountName = g.First().Designer!.Name,
					totalIncome = g.Sum(o => o.OrderPrice)
				})
				.OrderByDescending(x => x.totalIncome)
				.Take(topN)
				.Cast<object>()
				.ToList();

			return result;
		}

		public async Task<object> GetOrders()
		{
			var orders = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => !o.IsCart)
				.ToListAsync();

			// Build list of latest status per order
			var ordersWithLatestStatus = orders
				.Select(o =>
				{
					var latest = o.TrackingStatuses!
						.OrderByDescending(ts => ts.Time)
						.FirstOrDefault();

					return new
					{
						Month = latest?.Time.Month,
						StatusGroup = latest?.Status?.Name switch
						{
							"Pending" or "Processing" => "Processing",
							"Delivered" => "Delivered",
							"Cancelled" or "Refunded" => "Cancelled",
							_ => null
						}
					};
				})
				.Where(x => x.Month != null && x.StatusGroup != null)
				.ToList();

			var monthlyStatusCounts = Enumerable.Range(1, 12).Select(month =>
			{
				var monthOrders = ordersWithLatestStatus.Where(o => o.Month == month);

				return new
				{
					month = month,
					processing = monthOrders.Count(o => o.StatusGroup == "Processing"),
					delivered = monthOrders.Count(o => o.StatusGroup == "Delivered"),
					cancelled = monthOrders.Count(o => o.StatusGroup == "Cancelled")
				};
			}).ToList();

			return monthlyStatusCounts;
		}

		public async Task<object> GetMonthlyUserGrowth()
		{
			var currentYear = DateTime.Now.Year;

			var users = await _unitOfWork.GetRepository<Account>().Entities
				.Where(c => (c.Role == RoleEnum.Customer || c.Role == RoleEnum.Designer) && c.VerificationToken == null && c.CreatedTime.Year == currentYear)
				.ToListAsync();

			var result = Enumerable.Range(1, 12).Select(i =>
			{
				var month = i;
				var count = users.Count(c => c.CreatedTime.Month == month);
				return new
				{
					month = month,
					count = count
				};
			}).ToList();

			return result;
		}

		public async Task<object> GetRevenueByDayForDesigner(int month, int year)
		{
			string designerId = _authService.GetCurrentAccountId();

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer && a.DeletedBy == null).FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			var currentYear = DateTime.Now.Year;

			List<Order> orders = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => !o.IsCart && o.TrackingStatuses != null && o.DesignerId == designerId && o.TrackingStatuses.Any(ts => ts.StatusId == StatusEnum.Pending && ts.Time.Month == month && ts.Time.Year == year)).ToListAsync();

			int daysInMonth = DateTime.DaysInMonth(currentYear, month);


			var result = Enumerable.Range(1, daysInMonth)
				.Select(day => new
				{
					day,
					revenue = orders
						.Where(x =>
							x.TrackingStatuses?.FirstOrDefault(ts => ts.StatusId == StatusEnum.Pending)?.Time.Day == day)
						.Sum(x => x.OrderPrice)
				})
				.ToList();

			return result;
		}

		public async Task<object> GetTopSellingProducts(int topN)
		{
			string designerId = _authService.GetCurrentAccountId();

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer && a.DeletedBy == null).FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}


			var orders = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => !o.IsCart)
				.ToListAsync();

			var topProducts = orders
				.SelectMany(o => o.OrderDetails!)
				.Where(od => od.Product != null && od.Product.DesignerId == designerId)
				.GroupBy(od => new { od.ProductId, od.Product!.Name })
				.Select(g => new
				{
					productId = g.Key.ProductId,
					productName = g.Key.Name,
					quantitySold = g.Sum(od => od.Quantity)
				})
				.OrderByDescending(x => x.quantitySold)
				.Take(topN)
				.ToList();

			return topProducts;
		}

		public async Task<object> GetTopSellingProductsWithComments(int topN)
		{
			string designerId = _authService.GetCurrentAccountId();

			var designer = await _unitOfWork.GetRepository<Account>().Entities
				.FirstOrDefaultAsync(a =>
					a.Id == designerId &&
					a.Role == RoleEnum.Designer &&
					a.DeletedBy == null);

			if (designer == null)
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");

			var orders = await _unitOfWork.GetRepository<Order>().Entities
				.Where(o => !o.IsCart)
				.Include(o => o.OrderDetails!)
					.ThenInclude(od => od.Product)
						.ThenInclude(p => p.ProductReviews!)
							.ThenInclude(r => r.Customer)
				.ToListAsync();

			var topProducts = orders
				.SelectMany(o => o.OrderDetails!)
				.Where(od => od.Product != null && od.Product.DesignerId == designerId)
				.GroupBy(od => new { od.ProductId, od.Product!.Name })
				.Select(g =>
				{
					var product = g.First().Product!;

					return new
					{
						productId = g.Key.ProductId,
						productName = g.Key.Name,
						quantitySold = g.Sum(od => od.Quantity),
						Reviews = product.ProductReviews?
							.Select(pr => new
							{
								pr.Comment,
								pr.Star,
								Customer = new
								{
									Id = pr.Customer?.Id,
									Name = pr.Customer?.Name
								},
								Date = pr.Time.ToString("dd/MM/yyyy")
							})
							.OrderByDescending(pr => pr.Date)
							.ToList()
					};
				})
				.OrderByDescending(x => x.quantitySold)
				.Take(topN)
				.ToList();

			return topProducts;
		}

		public async Task<object> GetTotalProductReviews()
		{

			int totalReviews = await _unitOfWork.GetRepository<ProductReview>().Entities
				.CountAsync(r => r.Product != null && r.Product.DeletedBy == null);

			return new
			{
				totalReviews
			};
		}


	}
}
