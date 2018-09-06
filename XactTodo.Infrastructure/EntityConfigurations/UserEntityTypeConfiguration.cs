using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Domain.AggregatesModel.UserAggregate;

namespace XactTodo.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //builder.Ignore(b => b.DomainEvents);
            builder.HasIndex(p => new { p.UserName }).IsUnique();
            builder.HasIndex(p => new { p.Email }).IsUnique();
            builder.HasOne<User>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(nameof(User.CreatorUserId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除
        }
    }
}
