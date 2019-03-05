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
    class UserSettingEntityTypeConfiguration : IEntityTypeConfiguration<UserSetting>
    {
        public void Configure(EntityTypeBuilder<UserSetting> builder)
        {
            //builder.Ignore(b => b.DomainEvents);
            builder.Property(p=>p.Id).ValueGeneratedNever(); //设置主键不自动增长，以便指定用户ID值为主键值
        }
    }
}
