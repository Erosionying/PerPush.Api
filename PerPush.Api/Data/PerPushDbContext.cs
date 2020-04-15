using Microsoft.EntityFrameworkCore;
using PerPush.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Data
{
    public class PerPushDbContext:DbContext
    {
        public PerPushDbContext(DbContextOptions<PerPushDbContext> options):base(options)
        {

        }

        public DbSet<User> users { get; set; }
        public DbSet<Paper> papers { get; set; }

    }
}
