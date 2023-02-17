using ActivityApp.Application.ApplicationContracts.Responses;
using ActivityApp.Application.Core.Extensions;
using ActivityApp.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.Exceptions
{
    public static class ExceptionHelpers
    {
        public static ErrorResponse GetErrorResponse(this Exception ex, AspNetUsers user = null, string customMessage = null)
        {
            string errorMessage = null;
            int errorId = ex.Log(user);

            if (String.IsNullOrWhiteSpace(customMessage))
            {
                errorMessage = $"Something went wrong on the server, Please reach out to support and refer to this errorId: {errorId}";
            }
            else
            {
                errorMessage = customMessage + $" Please reach out to support and refer to this errorId: {errorId}";
            }

            return new ErrorResponse { APIErrors = new List<string> { errorMessage }, IsSuccess = false, ErrorID = errorId };
        }
    }
}
