using SnapRoom.Common.Base;

namespace SnapRoom.Contract.Services
{
	public interface ICategoryService
	{
		Task<BasePaginatedList<object>> GetCategories(bool? style, int pageNumber, int pageSize);
	}
}
