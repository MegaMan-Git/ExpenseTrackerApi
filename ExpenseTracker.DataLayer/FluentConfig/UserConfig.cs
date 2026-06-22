using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.DataLayer.Entities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.DataLayer.FluentConfig
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasKey(p => p.Id);
            builder
                .Property(p => p.FullName)
                .HasMaxLength(100)
                .IsRequired();
            builder
                .HasIndex(p => p.Email).IsUnique();
            builder
                .Property(p => p.Email)
                .HasMaxLength(60)
                .IsRequired();
            builder
                .Property(p => p.PasswordHash)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
