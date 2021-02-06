using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{
    /// <summary>
    /// 表示一段时间的值对象
    /// </summary>
    [Owned]
    public class PeriodOfTime: ValueObject
    {
        public decimal Num { get; set; }

        public TimeUnit Unit { get; set; }

        public PeriodOfTime(decimal num, TimeUnit unit)
        {
            Num = num;
            Unit = unit;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Num;
            yield return Unit;
        }
    }

    /// <summary>
    /// 时间单位
    /// </summary>
    public enum TimeUnit
    {
        /// <summary>
        /// 工作日
        /// </summary>
        Weekday = 0,

        /// <summary>
        /// 自然日
        /// </summary>
        NaturalDay = 1,

        /// <summary>
        /// 周
        /// </summary>
        Week = 7,

        /// <summary>
        /// 月
        /// </summary>
        Month = 30,

        /// <summary>
        /// 年
        /// </summary>
        Year = 365,
    }
}
