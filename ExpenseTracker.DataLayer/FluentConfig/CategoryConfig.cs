using ExpenseTracker.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.DataLayer.FluentConfig
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .HasKey(p => p.Id);
            builder
                .Property(p => p.Name).HasMaxLength(50).IsRequired();

            builder
                .HasOne(p => p.User)
                .WithMany(p => p.Categories)
                .HasForeignKey(p => p.UserId)
                //اگر رکورد اصلی حذف شد، تمام رکوردهای وابسته به آن را هم به‌ طور خودکار حذف کن
                .OnDelete(DeleteBehavior.Restrict);
            

        }

    }
}
