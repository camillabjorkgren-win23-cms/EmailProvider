using System;
using System.Collections.Generic;
using Azure.Communication.Email;
using EmailProvider.Models;
using EmailProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Functions;

public class EmailSender(ILoggerFactory loggerFactory, EmailService emailService)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<EmailSender>();
    private readonly EmailService _emailService = emailService;

    [Function("EmailSender")]
    public async Task Run([CosmosDBTrigger(
        databaseName: "OnatrixDatabase",
        containerName: "FormDatas",
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
                    if (document.Email != null)
                    {

                        var emailRequest = _emailService.GenerateEmailRequest(document);
                        if (emailRequest != null)
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
