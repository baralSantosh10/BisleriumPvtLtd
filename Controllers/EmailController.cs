using System.Net.Mail;
using System.Net;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MimeKit;

public class EmailController : Controller
{
    public IActionResult SendEmail()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SendEmail(string toEmail, string subject, string body)
    {
        if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
        {
            ModelState.AddModelError("", "Please provide all required fields.");
            return View(); // Consider returning appropriate view here.
        }

        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Sender Name", "sudipbhandari67@gmail.com"));
            email.To.Add(new MailboxAddress("Receiver Name", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("baralsantos10@gmail.com", "xyzkilluhh987065");
                smtp.Send(email);
                smtp.Disconnect(true);
            }

            return View("Index"); // Return success view
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            ModelState.AddModelError("", $"Failed to send email: {ex.Message}");
            return View(); // Consider returning appropriate view here.
        }

    }
}
 
