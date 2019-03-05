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
    class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            //builder.Ignore(b => b.DomainEvents);
            builder.HasIndex(p =>new { p.Name, p.CreatorUserId }).IsUnique().HasFilter("IsDeleted=0");
            var navigation = builder.Metadata.FindNavigation(nameof(Team.Members));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<User>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(nameof(Team.LeaderId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除
            builder.HasOne<User>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(nameof(Team.CreatorUserId))
                .OnDelete(DeleteBehavior.Restrict); //不级联删除

        }
    }
}
