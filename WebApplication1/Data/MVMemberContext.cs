using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

    public class MVMemberContext : DbContext
    {
        public MVMemberContext (DbContextOptions<MVMemberContext> options)
            : base(options)
        {
        }

        public DbSet<WebApplication1.Models.Member> Member { get; set; } = default!;
    }
