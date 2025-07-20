namespace SnapRoom.Contract.Services
{
	public interface IDashboardService
	{
		Task<object> GetRevenueByDay(int month, int year);
		Task<object> GetTopDesignersByRevenue(int topN);
		Task<object> GetOrders();
		Task<object> GetMonthlyUserGrowth();
		Task<object> GetRevenueByDayForDesigner(int month, int year);
		Task<object> GetTopSellingProducts(int topN);
		Task<object> GetTopSellingProductsWithComments(int topN);
		Task<object> GetTotalProductReviews();
	}
}
