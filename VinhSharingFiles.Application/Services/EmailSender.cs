using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace VinhSharingFiles.Application.Services
{
    public class EmailSender(IConfiguration configuration)
    {
        private readonly string _senderEmail = $"{configuration["GmailCredentials:SenderEmail"]}";
        private readonly string _appPassword = $"{configuration["GmailCredentials:AppPassword"]}";
        private readonly string _smtpAddress = $"{configuration["GmailCredentials:SmtpAddress"]}";        

        public void SendEmail(string recipientEmail, string subject, string body)
        {
            // Implement email sending logic here using SMTP or an email service provider
            try
            {
                // Create a new MailMessage object
                MailMessage mail = new()
                {
                    // Set sender and recipient
                    From = new MailAddress(_senderEmail) // Replace with your email address
                };
                mail.To.Add(recipientEmail);

                // Set subject and body
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Set to true if the body contains HTML content

                // Configure the SMTP client
                // Replace with your SMTP server address
                SmtpClient smtpClient = new(_smtpAddress)
                {
                    Port = 465, // Common port for TLS/SSL
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_senderEmail, _appPassword) // Replace with your email credentials
                }; 

                // Send the email
                smtpClient.Send(mail);

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}