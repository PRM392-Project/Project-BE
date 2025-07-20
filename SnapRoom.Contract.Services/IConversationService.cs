namespace SnapRoom.Contract.Services
{
	public interface IConversationService
	{
		Task<object> GetMessages(string id);
		Task<object> StartConversation(string receiverId);
		Task<object> GetConversations();
	}
}
