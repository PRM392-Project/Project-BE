using Microsoft.AspNetCore.Http;

namespace SnapRoom.Contract.Repositories.Dtos.ProductDtos
{
	public class DesignCreateDto
	{
		public string Name { get; set; } = default!;
		public decimal Price { get; set; }
		public string Description { get; set; } = default!;

		public bool Active { get; set; } = true;
		public string StyleId { get; set; } = default!;
		public List<string> CategoryIds { get; set; } = new List<string>();
		public IFormFile PrimaryImage { get; set; } = default!;
		public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
	}
}
