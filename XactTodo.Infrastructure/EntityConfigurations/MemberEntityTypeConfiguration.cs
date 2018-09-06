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
    class MemberEntityTypeConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            //builder.Ignore(b => b.DomainEvents);
            builder.HasOne<User>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(nameof(Member.UserId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除
        }
    }
}
