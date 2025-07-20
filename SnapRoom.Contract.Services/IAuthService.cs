using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Dtos.AccountDtos;

namespace SnapRoom.Contract.Services
{
	public interface IAuthService
	{
		Task<string> CustomerLogin(LoginDto loginDto);
		Task<string> DesignerLogin(LoginDto loginDto);
		Task CustomerRegister(RegisterDto registerDto);
		Task DesignerRegister(DesignerRegisterDto registerDto);
		Task SendApplicationResultEmail(string email, bool isApproved);
		Task VerifyAccount(string token);
		Task ForgetPassword(RoleEnum role, string email);
		Task VerifyResetPassowrd(string token);
		Task ResetPassword(string token, string newPassword);
		Task UpdatePassword(string password, string newPassword);
		Task UpdateEmail(string newEmail);
		Task ConfirmUpdateEmail(string otp);
		void UpdateAudits(BaseEntity entity, bool isCreating, bool isDeleting = false);
		string GetCurrentAccountId();
		string GetCurrentRole();

	}
}
