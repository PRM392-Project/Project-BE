using Microsoft.AspNetCore.Http;

namespace SnapRoom.Contract.Repositories.Dtos.ProductDtos
{
	public class DesignUpdateDto
	{
		public string? Name { get; set; } 
		public decimal? Price { get; set; }
		public string? Description { get; set; }

		public bool? Active { get; set; } = true;
		public string? StyleId { get; set; }
		public List<string>? CategoryIds { get; set; }
		public IFormFile? PrimaryImage { get; set; }
		public List<IFormFile>? Images { get; set; }

	}
}
