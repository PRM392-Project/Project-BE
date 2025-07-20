using Microsoft.AspNetCore.Mvc;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Services;
using SnapRoom.Services;

namespace SnapRoom.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConversationsController : ControllerBase
	{
		private readonly IConversationService _conversationService;

		public ConversationsController(IConversationService conversationService)
		{
			_conversationService = conversationService;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetMessages(string id)
		{
			var result = await _conversationService.GetMessages(id);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy đoạn chat thành công",
				data: result
			));
		}

		[HttpGet("receiver/{receiverId}")]
		public async Task<IActionResult> StartConversation(string receiverId)
		{
			var result = await _conversationService.StartConversation(receiverId);
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Bắt đầu đoạn hội thoại thành công",
				data: result
			));
		}


		[HttpGet]
		public async Task<IActionResult> GetConversations()
		{
			var result = await _conversationService.GetConversations();
			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy các đoạn hội thoại thành công",
				data: result
			));
		}

	}
}
