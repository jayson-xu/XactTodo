using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.UserAggregate
{
    /// <summary>
    /// 用户设置
    /// </summary>
    /// <remarks>其主键值即为用户Id，所以不需要再使用其他字段记录用户相关信息</remarks>
    public class UserSetting: BaseEntity
    {
    }
}
