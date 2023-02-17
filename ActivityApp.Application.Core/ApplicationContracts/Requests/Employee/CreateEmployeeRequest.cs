using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.Employee
{
    public class CreateEmployeeRequest : BaseRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]

        public string Email { get; set; }

        [Required]

        public DateTime DateOfBirth { get; set; }
        [Required]

        public string Department { get; set; }


    }
}
