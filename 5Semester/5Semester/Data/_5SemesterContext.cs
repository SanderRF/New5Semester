using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using _5Semester.Models;

namespace _5Semester.Models
{
    public class _5SemesterContext : DbContext
    {
        public _5SemesterContext (DbContextOptions<_5SemesterContext> options)
            : base(options)
        {
        }

        public DbSet<_5Semester.Models.Product> Product { get; set; }

        public DbSet<_5Semester.Models.User> User { get; set; }
    }
}
