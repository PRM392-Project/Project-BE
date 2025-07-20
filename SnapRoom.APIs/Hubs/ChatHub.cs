using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;
using static SnapRoom.Common.Base.BaseException;

namespace SnapRoom.APIs.Hubs
{
	public class ChatHub : Hub
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuthService _authService;

		public ChatHub(IUnitOfWork unitOfWork, IAuthService authService)
		{
			_unitOfWork = unitOfWork;
			_authService = authService;
		}

		// Client phải gọi hàm này sau khi kết nối
		public async Task JoinConversation(string conversationId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
			Console.WriteLine($"✅ Client {Context.ConnectionId} joined group {conversationId}");
		}


		public async Task SendMessage(string senderId, string receiverId, string content)
		{
			Account? sender = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == senderId).FirstOrDefaultAsync();

			if (sender == null) 
			{
				throw new ErrorException(400, "", "Lỗi tạo đoạn hội thoại");
			}

			// 1. Check if a conversation already exists between the sender and receiver
			var conversation = await _unitOfWork.GetRepository<Conversation>()
				.Entities
				.FirstOrDefaultAsync(c =>
					(c.CustomerId == senderId && c.DesignerId == receiverId) ||
					(c.CustomerId == receiverId && c.DesignerId == senderId));

			// 2. If not, create the conversation
			if (conversation == null)
			{
				conversation = new Conversation
				{
					CustomerId = (sender.Role == RoleEnum.Customer) ? senderId : receiverId, // <-- You may need logic to assign properly
					DesignerId = (sender.Role == RoleEnum.Customer) ? receiverId : senderId,
				};

				await _unitOfWork.GetRepository<Conversation>().InsertAsync(conversation);
			}

			// 3. Create the message
			var message = new Message
			{
				ConversationId = conversation.Id,
				SenderId = senderId,
				Content = content
			};

			await _unitOfWork.GetRepository<Message>().InsertAsync(message);
			await _unitOfWork.SaveAsync();

			// 4. Send message to SignalR group
			await Clients.Group(conversation.Id).SendAsync("ReceiveMessage", new
			{
				senderId,
				content,
				conversationId = conversation.Id,
				createdTime = message.CreatedTime.ToString("o")
			});
		}
	}
}
