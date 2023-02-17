
using ActivityApp.Application.Core.ApplicationContracts.Common;
using System;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.Example
{
    public class GetParticipantResponse : GenericResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
