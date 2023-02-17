﻿using Microsoft.AspNetCore.Identity;
using System;

namespace ActivityApp.Domain.Data
{
    public  class AspNetUsers:IdentityUser
    {
       
    
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int ClientId { get; set; }
        public DateTime? PasswordResetSentDate { get; set; }
        public string Code { get; set; }
        public bool IsAutoGeneratedLpn { get; set; }

       
    }
}
