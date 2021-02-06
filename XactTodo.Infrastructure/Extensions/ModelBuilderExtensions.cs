using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XactTodo.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        private const string prefixOfTable = "";

        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                //entity.SetTableName(prefixOfTable + entity.DisplayName()); //这种方式会导致值对象属性生成的表名非常奇怪，如：Matter.IntervalPeriod#PeriodOfTime
                //而EF框架为值对象属性生成的表名与实体类表名相同(都是复数)，所以取.号之前的一段作为表名以保持与在此设置的实体类表名一致
                entity.SetTableName(prefixOfTable + entity.DisplayName().Split('.').First());
            }
        }
    }
}
