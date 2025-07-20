using System.ComponentModel.DataAnnotations;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Image
	{
		[Key]
		public string ImageSource { get; set; } = default!;

		public virtual Account? Account { get; set; }
		public string? ProductId { get; set; }
		public virtual Product? Product { get; set; }
		public bool IsPrimary { get; set; } = false;
	}
}
