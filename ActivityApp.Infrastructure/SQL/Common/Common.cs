using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Infrastructure.SQL.Common
{
    public static class Common
    {
        public static DbContextOptionsBuilder<ApplicationDbContext> options;
    }
}
