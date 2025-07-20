using Microsoft.Extensions.Configuration;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Entities;
using System.Net;
using System.Net.Mail;

namespace SnapRoom.Services
{
	public class EmailService
	{
		private readonly IConfiguration _config;

		public EmailService(IConfiguration config)
		{
			_config = config;
		}

		public Task SendVerificationMail(Account account)
		{

			SmtpClient client = new SmtpClient(_config["SMTP:Server"], 587)
			{
				EnableSsl = true,
				Credentials = new NetworkCredential(_config["SMTP:Mail"], _config["SMTP:Password"])
			};

			string backendUrl = _config["BACKEND_URL"]!;

			string verificationUrl = $"{backendUrl}/api/auth/verify-account?token={account.VerificationToken}";
			string body = "";
			string imageUrl = "https://dataimage.blob.core.windows.net/snaproom/app-banner.png";


			if (account.Role == RoleEnum.Customer)
			{
				body = $@"
					<div style='font-family: Arial, sans-serif; line-height: 1.5;'>
						<div style='text-align: center; margin-bottom: 20px;'>
							<img src='{imageUrl}' alt='SnapRoom Banner' style='max-width: 100%; height: auto;' />
						</div>

						<p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>SnapRoom</strong>! Để hoàn tất quá trình đăng ký, vui lòng xác minh email của bạn bằng cách nhấn vào nút bên dưới:</p>

						<p style='text-align: center;'>
							<a href='{verificationUrl}' 
							   style='display: inline-block; padding: 10px 15px; background-color: #4CAF50; color: white;
									  text-decoration: none; border-radius: 5px; font-weight: bold;'>
								🔗 Nhấn vào đây để xác minh
							</a>
						</p>

						<p>Liên kết này sẽ hết hạn sau <strong>15 phút</strong>, vui lòng xác minh tài khoản sớm nhất có thể.</p>

						<p>Nếu bạn không yêu cầu tạo tài khoản, vui lòng bỏ qua email này.</p>

						<p>Trân trọng,<br><strong>Đội ngũ hỗ trợ SnapRoom</strong></p>
					</div>";

			}
			else
			{
				body = $@"
					<div style='font-family: Arial, sans-serif; line-height: 1.5;'>
						<div style='text-align: center; margin-bottom: 20px;'>
							<img src='{imageUrl}' alt='SnapRoom Banner' style='max-width: 100%; height: auto;' />
						</div>

						<p>Chào mừng bạn đến với <strong>SnapRoom</strong>!</p>

						<p>Chúng tôi đã nhận được yêu cầu đăng ký tài khoản <strong>nhà thiết kế</strong> cùng với hồ sơ cá nhân của bạn.</p>

						<p>Hiện tại, hồ sơ của bạn đang trong quá trình <strong>xét duyệt</strong> bởi đội ngũ kiểm duyệt của SnapRoom. Chúng tôi sẽ đánh giá các thông tin và đường dẫn hồ sơ bạn cung cấp để đảm bảo chất lượng và phù hợp với tiêu chí cộng đồng.</p>

						<p>Sau khi hoàn tất quá trình xét duyệt, bạn sẽ nhận được một email thông báo về kết quả đăng ký. Nếu được chấp thuận, bạn sẽ có thể truy cập vào tài khoản và bắt đầu chia sẻ các thiết kế của mình trên nền tảng SnapRoom.</p>

						<p><strong>Lưu ý:</strong> Quá trình xét duyệt có thể mất đến <strong>24–48 giờ</strong>.</p>

						<p>Chân thành cảm ơn bạn đã quan tâm và mong muốn trở thành một phần của cộng đồng thiết kế SnapRoom.</p>

						<p>Trân trọng,<br><strong>Đội ngũ SnapRoom</strong></p>
					</div>";
			}

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_config["SMTP:Mail"]!),
				Subject = "Xác minh tài khoản SnapRoom",
				Body = body,
				IsBodyHtml = true
			};

			// Thêm người nhận
			mailMessage.To.Add(account.Email);

			// Gửi email
			return client.SendMailAsync(mailMessage);
		}

		public Task SendResetPasswordEmail(string email, string resetToken)
		{

			SmtpClient client = new SmtpClient(_config["SMTP:Server"], 587)
			{
				EnableSsl = true,
				Credentials = new NetworkCredential(_config["SMTP:Mail"], _config["SMTP:Password"])
			};

			var backendUrl = _config["BACKEND_URL"];
			string resetUrl = $"{backendUrl}/api/auth/verify-reset-password?token={resetToken}";
			string imageUrl = "https://dataimage.blob.core.windows.net/snaproom/app-banner.png";


			var body = $@"
				<div style='font-family: Arial, sans-serif; line-height: 1.5;'>
					<div style='text-align: center; margin-bottom: 20px;'>
					<img src='{imageUrl}' alt='SnapRoom Banner' style='max-width: 100%; height: auto;' />
				</div>				

				<div style='font-family: Arial, sans-serif; line-height: 1.5;'>
					<p>Chúng tôi nhận được yêu cầu khôi phục mật khẩu cho tài khoản của bạn tại <strong>SnapRoom</strong>. Nếu bạn đã gửi yêu cầu này, vui lòng nhấn vào nút bên dưới để đặt lại mật khẩu:</p>

					<p style='text-align: center;'>
						<a href='{resetUrl}' 
						   style='display: inline-block; padding: 10px 15px; background-color: #FF5733; color: white;
								  text-decoration: none; border-radius: 5px; font-weight: bold;'>
							🔑 Đặt lại mật khẩu
						</a>
					</p>

					<p>Liên kết này sẽ hết hạn sau <strong>15 phút</strong>, vui lòng đặt lại mật khẩu trong thời gian sớm nhất có thể.</p>

					<p>Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này.</p>

					<p>Trân trọng,<br><strong>Bộ phận hỗ trợ SnapRoom</strong></p>
				</div>";

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_config["SMTP:Mail"]!),
				Subject = "Xác minh tài khoản Kids Vaccine",
				Body = body,
				IsBodyHtml = true
			};

			// Thêm người nhận
			mailMessage.To.Add(email);

			// Gửi email
			return client.SendMailAsync(mailMessage);
		}

		public Task SendUpdateEmailEmail(string newEmail, string otp)
		{
			SmtpClient client = new SmtpClient(_config["SMTP:Server"], 587)
			{
				EnableSsl = true,
				Credentials = new NetworkCredential(_config["SMTP:Mail"], _config["SMTP:Password"])
			};

			var backendUrl = _config["BACKEND_URL"];
			string updateEmailUrl = $"{backendUrl}/api/authentication/verify-reset-password?token={otp}";

			var body = $@"
				<div style='font-family: Arial, sans-serif; line-height: 1.5;'>
					<p>Bạn đã yêu cầu thay đổi địa chỉ email cho tài khoản của mình tại <strong>Kids Vaccine</strong>.</p>

					<p>Mã xác minh của bạn là:</p>

					<p style='text-align: center; font-size: 24px; font-weight: bold; background-color: #f3f3f3; padding: 10px; 
							  display: inline-block; border-radius: 5px;'>
						{otp}
					</p>

					<p>Vui lòng nhập mã này trong vòng <strong>15 phút</strong> để hoàn tất quá trình thay đổi email.</p>

					<p>Nếu bạn không yêu cầu thay đổi email, vui lòng bỏ qua email này.</p>

					<p>Trân trọng,<br><strong>Bộ phận hỗ trợ KVC</strong></p>
				</div>";
			var mailMessage = new MailMessage
			{
				From = new MailAddress(_config["SMTP:Mail"]!),
				Subject = "Xác minh tài khoản Kids Vaccine",
				Body = body,
				IsBodyHtml = true
			};

			// Thêm người nhận
			mailMessage.To.Add(newEmail);

			// Gửi email
			return client.SendMailAsync(mailMessage);

		}

		public Task SendApplicationResultMail(string email, bool isApproved)
		{
			SmtpClient client = new SmtpClient(_config["SMTP:Server"], 587)
			{
				EnableSsl = true,
				Credentials = new NetworkCredential(_config["SMTP:Mail"], _config["SMTP:Password"])
			};


			var body = "";

			if (isApproved)
			{
				body = @"
					<div style='font-family: Arial, sans-serif; line-height: 1.5;'>
						<h2>🎉 Chúc mừng!</h2>
						<p>Bạn đã được <strong>chấp thuận</strong> trở thành <strong>nhà thiết kế</strong> tại SnapRoom.</p>
						<p>Bạn có thể đăng nhập và bắt đầu chia sẻ những thiết kế tuyệt vời của mình ngay bây giờ.</p>
						<p>Chúng tôi rất mong được hợp tác cùng bạn.</p>
						<p>Thân ái,<br><strong>Đội ngũ SnapRoom</strong></p>
					</div>";
			}
			else
			{
				body = @"
					<div style='font-family: Arial, sans-serif; line-height: 1.5;'>
						<h2>🛑 Rất tiếc!</h2>
						<p>Sau khi xem xét hồ sơ của bạn, chúng tôi rất tiếc phải thông báo rằng yêu cầu đăng ký tài khoản <strong>nhà thiết kế</strong> tại SnapRoom đã <strong>không được chấp nhận</strong>.</p>
						<p>Bạn có thể xem xét và cập nhật lại hồ sơ của mình và thử lại sau.</p>
						<p>Chúng tôi trân trọng sự quan tâm của bạn đến nền tảng của chúng tôi.</p>
						<p>Thân ái,<br><strong>Đội ngũ SnapRoom</strong></p>
					</div>";
			}


			var mailMessage = new MailMessage
			{
				From = new MailAddress(_config["SMTP:Mail"]!),
				Subject = "Xác minh tài khoản SnapRoom",
				Body = body,
				IsBodyHtml = true
			};

			// Thêm người nhận
			mailMessage.To.Add(email);

			// Gửi email
			return client.SendMailAsync(mailMessage);

		}

	}
}
