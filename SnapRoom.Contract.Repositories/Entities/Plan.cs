using SnapRoom.Common.Base;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Plan : BaseEntity
	{
		public string Name { get; set; } = default!;
		public string Description { get; set; } = default!;

	}
}
