using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProjectOneMil.Models
{
	/// <summary>
	/// The SmtpEmailSender class is responsible for sending emails using SMTP protocol.
	/// </summary>
	public class SmtpEmailSender : IEmailSender
	{
		private string? _host;
		private int _port;
		private bool _enableSSL;
		private string? _username;
		private string? _password;

		/// <summary>
		/// Initializes a new instance of the SmtpEmailSender class with the specified SMTP server settings.
		/// </summary>
		/// <param name="host">The SMTP server host.</param>
		/// <param name="port">The SMTP server port.</param>
		/// <param name="enableSSL">Specifies whether SSL is enabled.</param>
		/// <param name="username">The username for the SMTP server.</param>
		/// <param name="password">The password for the SMTP server.</param>
		public SmtpEmailSender(string? host, int port, bool enableSSL, string? username, string? password)
		{
			_host = host;
			_port = port;
			_enableSSL = enableSSL;
			_username = username;
			_password = password;
		}

		/// <summary>
		/// Sends an email asynchronously.
		/// </summary>
		/// <param name="email">The recipient's email address.</param>
		/// <param name="subject">The subject of the email.</param>
		/// <param name="message">The body of the email.</param>
		/// <returns>A task that represents the asynchronous email sending operation.</returns>
		public Task SendEmailAsync(string email, string subject, string message)
		{
			var client = new SmtpClient(_host, _port)
			{
				Credentials = new NetworkCredential(_username, _password),
				EnableSsl = _enableSSL
			};

			return client.SendMailAsync(
				new MailMessage(_username ?? "", email, subject, message)
				{
					IsBodyHtml = true
				}
			);
		}
	}
}
