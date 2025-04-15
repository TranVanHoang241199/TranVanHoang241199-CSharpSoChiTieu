using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;
using System;
using System.IO;
using System.Threading;

namespace CSharpSoChiTieu.common
{
    public class GmailServiceHelper
    {
        private static readonly string[] Scopes = { GmailService.Scope.GmailSend };
        private static readonly string ApplicationName = "Your App Name";
        private static readonly string CredentialPath = "credentials.json"; // Đường dẫn tới file OAuth client JSON

        // Lấy dịch vụ Gmail
        public static GmailService GetGmailService()
        {
            UserCredential credential;

            // Lấy credential từ file
            using (var stream = new FileStream(CredentialPath, FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Tạo GmailService client
            return new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        // Gửi email qua Gmail API
        public static void SendEmail(GmailService service, string toEmail, string subject, string body)
        {
            var message = new Message
            {
                Raw = Base64UrlEncode(CreateEmailMessage(toEmail, subject, body))
            };

            service.Users.Messages.Send(message, "me").Execute();
        }

        // Tạo email MIME message
        private static MimeMessage CreateEmailMessage(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();

            // Đặt thông tin người gửi
            message.From.Add(new MailboxAddress("Your Name", "your-email@gmail.com"));

            // Đặt người nhận
            message.To.Add(new MailboxAddress("", toEmail));

            // Đặt chủ đề cho email
            message.Subject = subject;

            // Đặt nội dung email
            message.Body = new TextPart("html") { Text = body }; // Định dạng HTML cho nội dung email

            return message;
        }

        // Mã hóa email thành Base64Url
        private static string Base64UrlEncode(MimeMessage message)
        {
            using (var memoryStream = new MemoryStream())
            {
                message.WriteTo(memoryStream);
                var byteArray = memoryStream.ToArray();
                return Convert.ToBase64String(byteArray).Replace('+', '-').Replace('/', '_').Replace("=", "");
            }
        }
    }
}
