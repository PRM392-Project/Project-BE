using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapRoom.Contract.Repositories.Entities
{
	public class Status
	{
		[Key]
		public StatusEnum Id { get; set; }
		public string Name { get; set; } = default!;
	}
}
