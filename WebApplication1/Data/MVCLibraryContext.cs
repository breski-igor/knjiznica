using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

    public class MVCLibraryContext : DbContext
    {
        public MVCLibraryContext (DbContextOptions<MVCLibraryContext> options)
            : base(options)
        {
        }

        public DbSet<WebApplication1.Models.Book> Book { get; set; } = default!;
    }
