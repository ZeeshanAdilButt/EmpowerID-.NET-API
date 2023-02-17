using ActivityApp.Application.ApplicationContracts.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.ApplicationContracts.Common
{
    //change to generic
    //public class BaseResponse<T> : ErrorResponse
    public class GenericResponse : ErrorResponse
    {
        public GenericResponse()
        {
        }

        //T Data -- TODO: rethink
        //public T Data { get; set; }
    }
}
