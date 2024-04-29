using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
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
            return View(); // Return the same view with validation errors
        }
        Console.WriteLine(toEmail);
        Console.WriteLine(subject);

        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Sender Name", "baralsantos10@gmail.com"));
            email.To.Add(new MailboxAddress("Receiver Name", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("baralsantos10@gmail.com", "fwoizvvdwedvtbrn");
                smtp.Send(email);
                smtp.Disconnect(true);
            }

         
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ModelState.AddModelError("", $"Failed to send email: {ex.Message}");
            return View(); // Return the same view with error message
        }
    }
}
