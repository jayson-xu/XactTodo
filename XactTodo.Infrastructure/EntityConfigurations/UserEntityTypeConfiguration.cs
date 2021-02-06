using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.Utils;

namespace XactTodo.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //builder.Ignore(b => b.DomainEvents);
            builder.HasIndex(p => p.UserName).IsUnique().HasFilter("IsDeleted=0");
            builder.HasIndex(p => p.Email).IsUnique().HasFilter("IsDeleted=0");
            builder.HasOne<User>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(nameof(User.CreatorUserId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除
            builder.HasData(new User
            {
                Id = -1,
                UserName = "admin",
                Password= Hasher.HashPassword("admin"),
                DisplayName = "管理员",
                Email = "",
                EmailConfirmed = false,
                CreationTime = DateTime.Today
            });
        }
    }
}
