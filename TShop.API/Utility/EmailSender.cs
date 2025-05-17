using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace TShop.API.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587) // (والبورت بكون هاد مخصص للايميل (ابحث بتشوف gmail رح يكون من نوع 
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("amjadhmaidi@gmail.com", "vxio epds xsrg zdtd\r\n") // بعد ما نكتب اسم التطبيق gmail password manager الباسورد منجيبها من ال
            };

            return client.SendMailAsync(
                new MailMessage(from: "amjadhmaidi@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {
                    IsBodyHtml = true // HTML code تكون عشكل message سماح ال 
                });
        }
    }
}
