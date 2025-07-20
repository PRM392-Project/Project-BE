using SnapRoom.Common.Enum;

namespace SnapRoom.Contract.Repositories.Dtos.OrderDtos
{
	public class CartUpdateDto
	{
		public MethodEnum Method { get; set; }
		public string? Address { get; set; }
		public string? PhoneNumber { get; set; }
	}
}
