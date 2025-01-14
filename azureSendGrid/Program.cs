using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

class EmailSender
{
    private static string apiKey = "Dein-API-Schlüssel"; // API-Schlüssel von Azure SendGrid

    public static async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("deinEmail@example.com", "Absendername");
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

        var response = await client.SendEmailAsync(msg);
        Console.WriteLine($"Status Code: {response.StatusCode}");
    }
}
