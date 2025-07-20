using SnapRoom.Common.Enum;
using SnapRoom.Common.Utils;

namespace SnapRoom.Common.Base
{
	public class BaseResponse<T>
	{
		public T? Data { get; set; }
		public string? Message { get; set; }
		public StatusCodeEnum StatusCode { get; set; }
		public BaseResponse(StatusCodeEnum statusCode, T? data, string? message)
		{
			Data = data;
			Message = message;
			StatusCode = statusCode;
		}

		public BaseResponse(StatusCodeEnum statusCode, T? data)
		{
			Data = data;
			StatusCode = statusCode;
		}

		public BaseResponse(StatusCodeEnum statusCode, string? message)
		{
			Message = message;
			StatusCode = statusCode;
		}

		public static BaseResponse<T> OkResponse(T? data)
		{
			return new BaseResponse<T>(StatusCodeEnum.OK, data);
		}
		public static BaseResponse<T> OkResponse(string? mess)
		{
			return new BaseResponse<T>(StatusCodeEnum.OK, mess);
		}

        public static object? ErrorResponse(string v)
        {
            throw new NotImplementedException();
        }
    }
}
