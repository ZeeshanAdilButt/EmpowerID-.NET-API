using ActivityApp.Application.Core.ApplicationContracts.Common;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.Example
{
    public class CreateEmployeeResponse : GenericResponse
    {
        public CreateEmployeeResponse(int id)
        {
            Id = id;
        }

        public CreateEmployeeResponse()
        {

        }

        public int Id { get; set; }
       
    }
}
