using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ActivityApp.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Infrastructure.SQL
{

    public partial class ApplicationDbContext : IdentityDbContext<AspNetUsers>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
           
            base.OnModelCreating(builder);
        }
 
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }

    }
}
