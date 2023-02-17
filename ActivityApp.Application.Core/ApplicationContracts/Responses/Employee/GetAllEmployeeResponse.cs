using ActivityApp.Application.Core.ApplicationContracts.Common;
using System;
using System.Collections.Generic;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.Example
{
    public class GetAllEmployeeResponse : GenericResponse
    {
        public List<GetEmployeeResponse> Data;
    }
}
