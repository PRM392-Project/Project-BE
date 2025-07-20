using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Common.Utils;
using SnapRoom.Contract.Repositories.Dtos.AccountDtos;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SnapRoom.Common.Base.BaseException;

namespace SnapRoom.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly EmailService _mailService;

		public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, EmailService mailService)
		{
			_unitOfWork = unitOfWork;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
			_mailService = mailService;
		}

		public Task ConfirmUpdateEmail(string otp)
		{
			throw new NotImplementedException();
		}

		public async Task ForgetPassword(RoleEnum role, string email)
		{
			Account? account = await _unitOfWork.GetRepository<Account>().Entities.Where(a => a.Email == email && a.Role == role && a.DeletedBy == null).FirstOrDefaultAsync();

			if (account == null)
				throw new ErrorException(401, "unauthorized", "Không tìm thấy email trong hệ thống");

			account.ResetPasswordToken = Guid.NewGuid().ToString();

			await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
			await _unitOfWork.SaveAsync();
			await _mailService.SendResetPasswordEmail(email, account.ResetPasswordToken);
		}

		public string GetCurrentAccountId()
		{
			var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

			return GetAccountIdFromTokenHeader(token);
		}

		public string GetCurrentRole()
		{
			throw new NotImplementedException();
		}

		public async Task<string> CustomerLogin(LoginDto dto)
		{
			Account? account = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Email == dto.Email && a.DeletedBy == null && a.Role == RoleEnum.Customer)
				.FirstOrDefaultAsync();

			if (account == null || !BCrypt.Net.BCrypt.Verify(dto.Password, account.Password))
			{
				throw new ErrorException(401, "unauthorized", "Sai mật khẩu hoặc tài khoản");
			}

			if (account.VerificationToken != null)
			{
				throw new ErrorException(403, "forbidden", "Tài khoản chưa được xác thực, khách hàng vui lòng kiểm tra hộp mail");
			}


			return GenerateJwtToken(account);
		}

		public async Task<string> DesignerLogin(LoginDto dto)
		{
			Account? account = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Email == dto.Email && a.DeletedBy == null && (a.Role == RoleEnum.Designer || a.Role == RoleEnum.Admin))
				.FirstOrDefaultAsync();

			if (account == null || !BCrypt.Net.BCrypt.Verify(dto.Password, account.Password))
			{
				throw new ErrorException(401, "unauthorized", "Sai mật khẩu hoặc tài khoản");
			}

			if (account.VerificationToken != null)
			{
				throw new ErrorException(403, "forbidden", "Tài khoản chưa được xác thực, khách hàng vui lòng kiểm tra hộp mail");
			}


			return GenerateJwtToken(account);
		}

		public async Task CustomerRegister(RegisterDto registerDto)
		{
			// Check if the user already exists
			var existingCustomer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Email == registerDto.Email && a.DeletedBy == null & a.Role == RoleEnum.Customer)
				.FirstOrDefaultAsync();
			if (existingCustomer != null)
			{
				throw new ErrorException(409, "conflict", "Email này đã được sử dụng, vui lòng thử lại");
			}

			// Hash the password
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

			// Create new user entity
			Account newCustomer = new()
			{
				Name = registerDto.Name,
				Password = hashedPassword,
				Email = registerDto.Email,
				Role = RoleEnum.Customer,
				VerificationToken = Guid.NewGuid().ToString()
			};

			// Save account to the database
			await _unitOfWork.GetRepository<Account>().InsertAsync(newCustomer);
			await _unitOfWork.SaveAsync();


			await _mailService.SendVerificationMail(newCustomer);
		}

		public async Task DesignerRegister(DesignerRegisterDto dto)
		{
			// Check if the user already exists
			var existingDesigner = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Email == dto.Email && a.DeletedBy == null & a.Role == RoleEnum.Designer)
				.FirstOrDefaultAsync();
			if (existingDesigner != null)
			{
				throw new ErrorException(409, "conflict", "Email này đã được sử dụng, vui lòng thử lại");
			}

			// Hash the password
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

			// Create new user entity
			Account newDesigner = new()
			{
				Name = dto.Name,
				Password = hashedPassword,
				Email = dto.Email,
				ApplicationUrl = dto.ApplicationUrl,
				Role = RoleEnum.Designer,
				VerificationToken = Guid.NewGuid().ToString()
			};

			// Save account to the database
			await _unitOfWork.GetRepository<Account>().InsertAsync(newDesigner);
			await _unitOfWork.SaveAsync();

			await _mailService.SendVerificationMail(newDesigner);

		}

		public async Task SendApplicationResultEmail(string email, bool isApproved)
		{
			Account designer = _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Email == email && a.DeletedBy == null && a.Role == RoleEnum.Designer && a.VerificationToken != null)
				.FirstOrDefault() ?? throw new ErrorException(404, "not_found", "Email không hợp lệ");

			if (isApproved)
			{
				designer.VerificationToken = null; // Approve the designer by removing the verification token
			}
			else
			{
				_unitOfWork.GetRepository<Account>().Delete(designer);
			}
			await _unitOfWork.SaveAsync();

			await _mailService.SendApplicationResultMail(email, isApproved);

		}

		public async Task ResetPassword(string token, string newPassword)
		{
			Account? account = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.ResetPasswordToken == token && a.DeletedBy == null).FirstOrDefaultAsync();

			if (account == null)
				throw new ErrorException(502, "bad_gateway", "Đường dẫn đổi mật khẩu đã hết hạn, vui lòng thử lại sau");

			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
			account.Password = hashedPassword;
			account.ResetPasswordToken = null;

			await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
			await _unitOfWork.SaveAsync();
		}

		public void UpdateAudits(BaseEntity entity, bool isCreating, bool isDeleting = false)
		{
			throw new NotImplementedException();
		}

		public Task UpdateEmail(string newEmail)
		{
			throw new NotImplementedException();
		}

		public async Task UpdatePassword(string password, string newPassword)
		{
			string accountId = GetCurrentAccountId();

			Account? account = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == accountId && a.DeletedBy == null).FirstOrDefaultAsync();

			if (account == null)
				throw new ErrorException(401, "unauthorized", "Không tìm thấy account id.");

			if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
			{
				throw new ErrorException(401, "unauthorized", "Mật khẩu hiện tại không chính xác. Vui lòng thử lại.");
			}

			// Hash the password
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

			account.Password = hashedPassword;

			await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
			await _unitOfWork.SaveAsync();
		}

		public async Task VerifyAccount(string token)
		{
			Account? account = _unitOfWork.GetRepository<Account>().Entities.Where(a => a.VerificationToken == token && a.DeletedBy == null).FirstOrDefault();

			if (account == null)
			{
				throw new ErrorException(502, "bad_gateway", "Token is not valid or is expired");
			}
			account.VerificationToken = null;

			await _unitOfWork.SaveAsync();
		}

		public async Task VerifyResetPassowrd(string token)
		{
			Account? account = await _unitOfWork.GetRepository<Account>().Entities.Where(a => a.ResetPasswordToken == token).FirstOrDefaultAsync();

			if (account == null)
			{
				throw new ErrorException(502, "bad_gateway", "Đường dẫn đổi mật khẩu đã hết hạn, vui lòng thử lại sau");
			}
		}

		private ClaimsPrincipal DecodeJwtToken(string token)
		{
			// Retrieve the JWT secret from configuration
			var secret = _configuration["JwtSettings:Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

			// Set up token validation parameters
			var tokenHandler = new JwtSecurityTokenHandler();
			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = key,
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true
			};

			try
			{
				// Validate the token and return the claims principal
				var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
				return principal;
			}
			catch (SecurityTokenExpiredException)
			{
				throw new SecurityTokenException("Token has expired");
			}
			catch (SecurityTokenInvalidSignatureException)
			{
				throw new SecurityTokenException("Invalid token signature");
			}
			catch (Exception)
			{
				throw new SecurityTokenException("Invalid token");
			}
		}

		private string GetAccountIdFromTokenHeader(string? token)
		{
			// Check if the token is null or empty
			if (string.IsNullOrEmpty(token))
			{
				return string.Empty; // Handle null or empty token gracefully
			}

			// Decode the JWT token and extract claims
			var principal = DecodeJwtToken(token);

			if (principal == null)
			{
				return string.Empty; // Handle null principal gracefully
			}

			// Extract claims from the principal
			var accountIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "Id");

			if (accountIdClaim != null && Guid.TryParse(accountIdClaim.Value, out Guid parsedAccountId))
			{
				return parsedAccountId.ToString().ToUpper();
			}

			return string.Empty;
		}

		private string GenerateJwtToken(Account account)
		{
			if (account == null)
			{
				throw new ArgumentNullException(nameof(account), "User object cannot be null.");
			}

			// Retrieve the JWT secret from configuration
			var secret = _configuration["JwtSettings:Secret"];
			if (string.IsNullOrEmpty(secret))
			{
				throw new ArgumentNullException("JwtSettings:Secret", "JWT Secret not found in configuration.");
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// Create claims based on user information, with null checks
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, account.Id),
				new Claim("Id", account.Id),
				new Claim("Name", account.Name),
				new Claim("Email", account.Email ?? ""),
				new Claim("Role", account.Role.ToString()),
				new Claim("PlanId", account.PlanId ?? ""),
				new Claim("AvatarSource", account.AvatarSource ?? ""),
				new Claim("ApplicationUrl", account.ApplicationUrl ?? "")
			};

			// Retrieve the token expiry period from configuration, handle parsing errors
			if (!int.TryParse(_configuration["JwtSettings:ExpiryInDays"], out var expiryInDays))
			{
				expiryInDays = 1; // Default to 1 day if parsing fails or value is not set
			}

			// Create and return the JWT token
			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.UtcNow.AddDays(expiryInDays),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}

}
