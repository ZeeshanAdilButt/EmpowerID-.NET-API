using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Domain.Data
{
    public partial class ErrorLog
    {
        public int Id { get; set; }
        public string ErrorText { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public int? ClientId { get; set; }
        public string UserId { get; set; }
        public virtual AspNetUsers CreatedByNavigation { get; set; }
    }
}
