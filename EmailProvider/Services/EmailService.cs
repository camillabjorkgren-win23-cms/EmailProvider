using Azure;
using Azure.Communication.Email;
using EmailProvider.Models;
using Microsoft.Extensions.Logging;


namespace EmailProvider.Services;
public class EmailService(ILogger<EmailService> logger, EmailClient emailClient)
{
    private readonly ILogger<EmailService> _logger = logger;
    private readonly EmailClient _emailClient = emailClient;
    public EmailRequest GenerateEmailRequest(EmailDocument document)
    {
        try
        {
            if (!string.IsNullOrEmpty(document.Email) && !string.IsNullOrEmpty(document.Name))
            {
                var emailRequest = new EmailRequest()
                {
                    To = document.Email,
                    Subject = $"Confirmation email",
                    HtmlBody = $@"
                        <html lang ='en'>
                            <head> 
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Confirmation Email</title>
                            </head>
                        <body>
                            <div style='color:#191919; max-width:500px'>
                                <div style='background-color:  #4F5955; color: white; text-align: center; padding: 20px 0;'>
                                    <h1 style='font-weight: 400;'>Thank you for your email</h1>
                                </div>
                                <div style='background-color: #f7f7f7; padding: 1rem 2rem;'>
                                    <p> Dear {document.Name},</p>
                                    <p> We have received your request through our website Onatrix. We will get back to you shortly</p>
                                   
                                   
                                </div>
                            <div style='color: #191919; text-align:center; font-size: 11px;'>
                                <p> Onatrix, Klarabergsviadukten 90, SE-123 45 Stockholm, Sweden</p>
                            </div>
                        </div>
                    </body>
                </html>
                        ",
                    PlainText = $"Thank you {document.Name}, we have received your request through Onatrix.com. We will get back to you shortly. This email can not receive replies. For more information, see our website Onatrix.com"
                };
                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.GenerateEmailRequest() :: {ex.Message}");
        }
        return null!;
    }

    public bool SendEmail(EmailRequest emailRequest)
    {
        try
        {
            var result = _emailClient.Send(
                WaitUntil.Completed,

                senderAddress: Environment.GetEnvironmentVariable("SenderAddress"),
                recipientAddress: emailRequest.To,
                subject: emailRequest.Subject,
                htmlContent: emailRequest.HtmlBody,
                plainTextContent: emailRequest.PlainText);

            if (result.HasCompleted)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.SendEmail() :: {ex.Message}");
        }
        return false;
    }
}
