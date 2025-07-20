namespace SnapRoom.Contract.Services
{
	public interface IPaymentService
	{
		Task<string> PayCart();
	}
}
