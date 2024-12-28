using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

    public class MVCOrdersContext : DbContext
    {
        public MVCOrdersContext (DbContextOptions<MVCOrdersContext> options)
            : base(options)
        {
        }

        public DbSet<WebApplication1.Models.Order> Order { get; set; } = default!;
    }
