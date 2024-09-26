using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Department> Departments { get; set; } = null;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasKey(d => d.Id); // Установка первичного ключа

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Parent) // Устанавливаем отношение к родительскому подразделению
                .WithMany(d => d.Children) // Устанавливаем отношение к дочерним подразделениям
                .HasForeignKey(d => d.ParentId) // Указываем внешний ключ
                .OnDelete(DeleteBehavior.Cascade); // Поведение при удалении
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=outdb;Username=postgres;Password=1234");
        }
    }
}