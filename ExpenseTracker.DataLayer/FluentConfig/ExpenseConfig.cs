using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.DataLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.DataLayer.FluentConfig
{
    public class ExpenseConfig : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder.Property(p => p.Amount).IsRequired();
            builder.Property(p => p.Title).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Date).IsRequired();
            builder.Property(p => p.Note).HasMaxLength(300);
            builder.Property(p => p.CategoryName).IsRequired().HasMaxLength(50);

            //Add Relation
            builder
                .HasOne(p => p.User)
                .WithMany(p => p.Expenses)
                .HasForeignKey(p => p.UserId);
            builder
                .HasOne(p => p.Category)
                .WithMany(p => p.Expenses)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
