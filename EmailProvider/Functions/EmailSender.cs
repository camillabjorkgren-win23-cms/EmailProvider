using System;
using System.Collections.Generic;
using Azure.Communication.Email;
using EmailProvider.Models;
using EmailProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Functions;

public class EmailSender(ILoggerFactory loggerFactory, EmailClient emailClient)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<EmailSender>();
    private readonly EmailClient _emailClient = emailClient;
    private readonly EmailService _emailService;

    [Function("EmailSender")]
    public async Task Run([CosmosDBTrigger(
        databaseName: "OnatrixDatabase",
        containerName: "EmailRequests",
        Connection = "Cosmos_Db_ConnectionString",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<EmailDocument> documents)
    {
        try
        {
            if (documents != null && documents.Count > 0)
            {
                _logger.LogInformation("Documents modified: " + documents.Count);


                foreach (var document in documents)
                {
                    var emailDocument = JsonConvert.DeserializeObject<EmailDocument>(document.ToString());

                    if (emailDocument != null)
                    {
                        var emailRequest = _emailService.GenerateEmailRequest(document);
                        if(emailRequest != null)
                        {
                            bool success = _emailService.SendEmail(emailRequest);

                            if (success)
                            {
                                _logger.LogInformation("Email sent successfully");
                            }
                            else
                            {
                                _logger.LogError("Failed to send email");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.Run :: {ex.Message}");
        }
    }
}
