using EmailProvider.Contexts;
using EmailProvider.Entities;
using EmailProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace EmailProvider.Functions
{
    public class FormDataSaver(ILogger<FormDataSaver> logger, DataContext dataContext)
    {
        private readonly ILogger<FormDataSaver> _logger = logger;
        private readonly DataContext _dataContext = dataContext;

        [Function("FormDataSaver")]
        public async Task<bool> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if (body == null)
                {
                    _logger.LogError("Request data could not be read");
                    return false;
                }
                else
                {
                    try
                    {
                        var formData = JsonConvert.DeserializeObject<FormData>(body);
                        if (formData != null && formData.Email != null)
                        {
                            var formType = GenerateFormType(formData);

                            var formDataEntity = new FormDataEntity
                            {
                                Email = formData.Email,
                                Subject = formData.Subject,
                                Message = formData.Message,
                                Phone = formData.Phone,
                                PartitionKey = formType,
                                Name = formData.Name,
                            };
                            _dataContext.FormDatas.Add(formDataEntity);
                            await _dataContext.SaveChangesAsync();
                            _logger.LogInformation("Form data saved successfully");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ERROR : FormDataSaver.Run() :: {ex.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : FormDataSaver.Run() :: {ex.Message}");
            }
            return false;      
        }

        public string GenerateFormType(FormData formData)
        {
            var formType = "emailOnly";
            if (formData.Subject != null && formData.Name != null && formData.Message == null)
            {
                formType = "subjectForm";
                return formType;
            }
            if (formData.Message != null)
            {
                formType = "messageForm";
                return formType;
            }
            if (formData.Message == null && formData.Subject == null)
            {
                formType = "emailOnly";
                return formType;
            }
            return formType;
            
        }
    }
}
