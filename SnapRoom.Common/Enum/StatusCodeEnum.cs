using SnapRoom.Common.Utils;

namespace SnapRoom.Common.Enum
{
	public enum StatusCodeEnum
	{
		[CustomName("Success")]
		OK = 200,

        [CustomName("Bad Request")]
		BadRequest = 400,

        [CustomName("Unauthorized")]
		Unauthorized = 401,

        [CustomName("Internal Server Error")]
		ServerError = 500

	}
}
