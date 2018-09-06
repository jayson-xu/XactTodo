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
    class EvolvementEntityTypeConfiguration : IEntityTypeConfiguration<Evolvement>
    {
        public void Configure(EntityTypeBuilder<Evolvement> builder)
        {
            //builder.Ignore(b => b.DomainEvents);
            //builder.HasIndex(p=> p.CreatorUserId); //不必，创建外键会自动创建索引
            builder.HasOne<User>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(nameof(FullAuditedEntity.CreatorUserId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除
        }
    }
}
