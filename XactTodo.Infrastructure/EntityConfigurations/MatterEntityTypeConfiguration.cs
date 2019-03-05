using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Infrastructure.EntityConfigurations
{
    class MatterEntityTypeConfiguration : IEntityTypeConfiguration<Matter>
    {
        public void Configure(EntityTypeBuilder<Matter> builder)
        {
            //builder.Ignore(b => b.DomainEvents);
            builder.OwnsOne(o => o.EstimatedTimeRequired).Property(p=>p.Num).HasColumnType("decimal(9,1)");
            builder.OwnsOne(o => o.IntervalPeriod).Property(p=>p.Num).HasColumnType("decimal(9,1)");
            builder.HasIndex(p => p.Subject);
            builder.HasIndex(p => p.Importance);
            builder.HasIndex(p => p.Deadline);
            builder.HasIndex(p=> p.TeamId );
            builder.HasIndex(p=> p.CreatorUserId );
            /* 不再对Subject做唯一索引，因为不允许与很久以前已完成的事项主题重复会不太方便，考虑在界面上对主题重复进行提示即可
            builder.HasIndex(p => new { p.Subject, p.TeamId });
            //.IsUnique()
            //.HasFilter("IsDeleted=0 AND TeamId is not null"); //MySQL不支持带条件的唯一索引:(
            builder.HasIndex(p => new { p.Subject, p.CreatorUserId });
                //.IsUnique()
                //.HasFilter("IsDeleted=0 AND TeamId is null"); //MySQL不支持带条件的唯一索引:(
            */

            builder.HasOne<User>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(nameof(FullAuditedEntity.CreatorUserId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除

            builder.HasOne<Team>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(nameof(Matter.TeamId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除

        }
    }
}
