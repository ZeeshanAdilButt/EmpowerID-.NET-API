using Microsoft.Extensions.Configuration;
using ActivityApp.Domain.Data;
using ActivityApp.Infrastructure.SQL;
using ActivityApp.Infrastructure.SQL.Repostiories.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static int Log(this Exception e, AspNetUsers user = null, bool sendEmail = false, IConfiguration configuration = null)
        {
            if (configuration != null && configuration["Environment"] == "dev")
            {
                sendEmail = false;
            }

            var options = ActivityApp.Infrastructure.SQL.Common.Common.options;

            var _errorlogRepository = new GenericRepository<ErrorLog>(new ApplicationDbContext(options.Options));

            string Description = string.Empty;

            if (!string.IsNullOrEmpty(Convert.ToString(e.InnerException)))
            {
                Description = " InnerException: " + e.InnerException;
            }
            if (!string.IsNullOrEmpty(Convert.ToString(e.Message)))
            {
                if (!string.IsNullOrEmpty(Description))
                {
                    Description += Environment.NewLine;
                }
                Description += " Message: " + e.Message;
            }
            if (!string.IsNullOrEmpty(Convert.ToString(e.StackTrace)))
            {
                if (!string.IsNullOrEmpty(Description))
                {
                    Description += Environment.NewLine;
                }
                Description += " StackTrace: " + e.StackTrace;
            }

            ErrorLog errorLog = new ErrorLog { ErrorText = Description, CreateDate = DateTime.UtcNow, CreatedBy = user?.Id, ClientId = user?.ClientId == 0 ? null : user?.ClientId };

            _errorlogRepository.Insert(errorLog);

            if (sendEmail)
            {
                string emailContent = ("Dear Steve ,<br/><br/>" + Description + "<br/><br/>" + "Regards <br/>Support Team");
                string subject = ("Erorr Occured on the project");

                //var _emailSender = new EmailSender();
            }

            return errorLog.Id;
        }
    }
}
