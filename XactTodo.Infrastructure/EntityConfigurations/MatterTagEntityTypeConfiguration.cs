using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Domain.AggregatesModel.MatterTagAggregate;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Infrastructure.EntityConfigurations
{
    class MatterTagEntityTypeConfiguration : IEntityTypeConfiguration<MatterTag>
    {
        public void Configure(EntityTypeBuilder<MatterTag> builder)
        {
            builder.HasIndex(p => new { p.MatterId, p.UserId, p.Tag }).IsUnique();
            //builder.Ignore(b => b.DomainEvents);
            builder.HasOne<User>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(nameof(MatterTag.UserId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除

            builder.HasOne<Matter>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(nameof(MatterTag.MatterId));
        }
    }
}
