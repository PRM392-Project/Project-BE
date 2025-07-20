namespace SnapRoom.Common.Base
{
	public class BasePaginatedList<T>
	{
		public IEnumerable<T> Items { get; private set; }

		// Thuộc tính để lưu trữ tổng số phần tử
		public int TotalItems { get; private set; }

		// Thuộc tính để lưu trữ số trang hiện tại
		public int CurrentPage { get; private set; }

		// Thuộc tính để lưu trữ tổng số trang
		public int TotalPages { get; private set; }

		// Thuộc tính để lưu trữ số phần tử trên mỗi trang
		public int PageSize { get; private set; }

		// Constructor để khởi tạo danh sách phân trang
		public BasePaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
		{
			Items = new List<T>();
			TotalItems = count;

			if (ShouldReturnAll(pageNumber, pageSize, count))
			{
				ApplyFullList(items);
			}
			else
			{
				ApplyPagedList(items, pageNumber, pageSize);
			}
		}

		// Phương thức để kiểm tra nếu có trang trước đó
		public bool HasPreviousPage => CurrentPage > 1;

		// Phương thức để kiểm tra nếu có trang kế tiếp
		public bool HasNextPage => CurrentPage < TotalPages;

		private bool ShouldReturnAll(int pageNumber, int pageSize, int count)
		{
			return pageNumber <= -1 || pageSize <= -1 || pageSize == 0 || count == 0;
		}

		private void ApplyFullList(IReadOnlyCollection<T> items)
		{
			CurrentPage = 1;
			PageSize = TotalItems;
			TotalPages = 1;
			Items = items;
		}

		private void ApplyPagedList(IReadOnlyCollection<T> items, int pageNumber, int pageSize)
		{
			CurrentPage = pageNumber;
			PageSize = pageSize;
			TotalPages = (int)Math.Ceiling(TotalItems / (double)pageSize);
			Items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize);
		}
	}
}
