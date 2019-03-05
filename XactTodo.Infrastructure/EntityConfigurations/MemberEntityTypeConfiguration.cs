using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
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
            builder.HasIndex(p => new { p.TeamId, p.UserId}).IsUnique().HasFilter("IsDeleted=0");
            //不能在模型类声明外键的同时指定不级联删除，所以通过下面的代码设定，实现当用户还是某个小组的成员时则不能删除该用户
            builder.Metadata.GetForeignKeys().Where(p => p.PrincipalEntityType.Name == typeof(User).FullName)
                .First().DeleteBehavior = DeleteBehavior.Restrict;
            builder.Metadata.GetForeignKeys().Where(p => p.PrincipalEntityType.Name == typeof(Team).FullName)
                .First().DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
