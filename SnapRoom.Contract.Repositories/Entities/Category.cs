using SnapRoom.Common.Base;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Category : BaseEntity
	{
		public string Name { get; set; } = default!;
		public bool Style { get; set; } = false;
	}
}
