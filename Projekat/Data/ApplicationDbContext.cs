using Microsoft.EntityFrameworkCore;
using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Predmet> Predmeti { get; set; }
        public DbSet<Ispit> Ispiti { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ispit>()
                .HasOne(i => i.Student)
                .WithMany(s => s.Ispiti)
                .HasForeignKey(i => i.StudentID);

            modelBuilder.Entity<Ispit>()
                .HasOne(i => i.Predmet)
                .WithMany(p => p.Ispiti)
                .HasForeignKey(i => i.PredmetID);
        }
    }
}
