using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Design
	{
		[Key]
		[Column(TypeName = "nvarchar(36)")]
		public string Id { get; set; } = Guid.NewGuid().ToString().ToUpper();
		public virtual Product? Product { get; set; }

	}
}
