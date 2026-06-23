using ExpenseTracker.DataLayer.Entities;
using ExpenseTracker.DataLayer.FluentConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.DataLayer.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new ExpenseConfig());

            #region Seed Data
            var userId = new Guid("11111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = userId,
                    FullName = "Test User",
                    Email = "test@test.com",
                    PasswordHash = "StaticPasswordHashForSeedData",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0)
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Food",
                    UserId = userId
                },
                new Category
                {
                    Id = 2,
                    Name = "Transport",
                    UserId = userId
                }
            );

            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = 1,
                    Title = "Lunch",
                    Amount = 12.50m,
                    Date = new DateTime(2026, 1, 10),
                    Note = "Restaurant",
                    CreatedAt = new DateTime(2026, 1, 11),
                    CategoryId = 1,
                    UserId = userId,
                    CategoryName = "Food"
                },
                new Expense
                {
                    Id = 2,
                    Title = "Taxi",
                    Amount = 8.75m,
                    Date = new DateTime(2026, 1, 11),
                    Note = "City ride",
                    CreatedAt = new DateTime(2026, 1, 11),
                    CategoryId = 2,
                    UserId = userId,
                    CategoryName = "Transport"
                }
            );
            #endregion

        }
    }
}
