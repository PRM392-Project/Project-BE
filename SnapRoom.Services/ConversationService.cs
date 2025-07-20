using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;
using System.ComponentModel.DataAnnotations;
using static SnapRoom.Common.Base.BaseException;

namespace SnapRoom.Services
{
	public class ConversationService : IConversationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IAuthService _authService;

		public ConversationService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_authService = authService;
		}

		public async Task<object> GetMessages(string id)
		{
			var messages = await _unitOfWork.GetRepository<Message>().Entities
				.Where(m => m.ConversationId == id)
				.OrderBy(m => m.CreatedTime)
				.Select(m => new
				{
					m.SenderId,
					SenderAvatar = m.Sender.AvatarSource,
					m.Content,
					m.ConversationId,
					CreatedTime = m.CreatedTime.ToString("o") // Send as string for JSON compatibility
				})
				.ToListAsync();

			return messages;
		}

		public async Task<object> StartConversation(string receiverId)
		{
			string userId = _authService.GetCurrentAccountId();

			Account? user = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == userId).FirstOrDefaultAsync();

			if (user == null)
			{
				throw new ErrorException(404, "", "Tài khoản không tồn tại");
			}

			Conversation? conversation = await _unitOfWork.GetRepository<Conversation>().Entities
				.Where(c => (c.DesignerId == userId && c.CustomerId == receiverId) || (c.DesignerId == receiverId && c.CustomerId == userId))
				.FirstOrDefaultAsync();


			if (conversation == null)
			{
				conversation = new Conversation
				{
					CustomerId = (user.Role == RoleEnum.Customer) ? userId : receiverId,
					DesignerId = (user.Role == RoleEnum.Customer) ? receiverId : userId
				};

				Message initialMessage = new Message
				{
					SenderId = receiverId,
					Content = "Bạn đã liên hệ với nhà thiết kế.\nChúng tôi sẽ phản hồi sớm nhất có thể. (Đây là tin nhắn tự động)",
					ConversationId = conversation.Id
				};

				await _unitOfWork.GetRepository<Conversation>().InsertAsync(conversation);
				await _unitOfWork.GetRepository<Message>().InsertAsync(initialMessage);
				await _unitOfWork.SaveAsync();
			}



			return conversation.Id;
		}



		public async Task<object> GetConversations()
		{
			string userId = _authService.GetCurrentAccountId();

			Account? user = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == userId).FirstOrDefaultAsync();

			if (user == null)
			{
				throw new ErrorException(404, "", "Tài khoản không tồn tại");
			}

			var conversations = await _unitOfWork.GetRepository<Conversation>().Entities
				.Where(c => c.DesignerId == userId || c.CustomerId == userId)
				.Select(c => new
				{
					c.Id,
					LastMessageEntity = c.Messages!
						.OrderByDescending(m => m.CreatedTime)
						.FirstOrDefault(),
					Sender = new
					{
						Name = c.DesignerId == userId ? c.Customer!.Name : c.Designer!.Name,
						Avatar = c.DesignerId == userId ? c.Customer!.AvatarSource : c.Designer!.AvatarSource
					}
				})
				.OrderByDescending(c => c.LastMessageEntity!.CreatedTime)
				.Select(c => new
				{
					c.Id,
					LastMessage = c.LastMessageEntity!.Content,
					LastMessageTime = c.LastMessageEntity!.CreatedTime.ToString("o"),
					c.Sender
				})
				.ToListAsync();
			return conversations;
		}
	}
}
