using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ActivityApp.Domain.Interfaces;

namespace ActivityApp.Domain.Data
{
    public partial class Employee : ISoftDelete
    {
        public Employee()
        {
            IsActive= true;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Department { get; set; }
        public bool IsActive { get; set; }
    }
}
