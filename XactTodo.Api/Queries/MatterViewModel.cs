using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Queries
{
    /// <summary>
    /// 事项
    /// </summary>
    public class Matter
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 重要性
        /// </summary>
        public int Importance { get; private set; }

        /// <summary>
        /// 预计需时 数量
        /// </summary>
        public decimal EstimatedTimeRequired_Num { get; private set; }

        /// <summary>
        /// 预计需时 单位(枚举)
        /// </summary>
        public int EstimatedTimeRequired_Unit { get; private set; }

        /// <summary>
        /// 预计需时描述
        /// </summary>
        public string EstimatedTimeRequiredDescr
        {
            get
            {
                if (EstimatedTimeRequired_Num == 0)
                    return null;
                var unit = (Domain.AggregatesModel.MatterAggregate.TimeUnit)EstimatedTimeRequired_Unit;// Enum.GetName(typeof(Domain.AggregatesModel.MatterAggregate.TimeUnit), EstimatedTimeRequired_Unit);
                return $"{EstimatedTimeRequired_Num}{unit}(s)";
            }
        }

        /// <summary>
        /// 最后期限
        /// </summary>
        public DateTime? Deadline { get; private set; }

        /// <summary>
        /// 已完成
        /// </summary>
        public bool Finished { get; private set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishTime { get; private set; }

        /// <summary>
        /// 周期性事项
        /// </summary>
        public bool Periodic { get; private set; }

        /// <summary>
        /// 间隔周期 数量
        /// </summary>
        public decimal IntervalPeriod_Num { get; private set; }

        /// <summary>
        /// 间隔周期 单位(枚举)
        /// </summary>
        public decimal IntervalPeriod_Unit { get; private set; }

        /// <summary>
        /// 间隔周期描述
        /// </summary>
        public string IntervalPeriodDescr
        {
            get
            {
                if (IntervalPeriod_Num == 0)
                    return null;
                var unit = (Domain.AggregatesModel.MatterAggregate.TimeUnit)IntervalPeriod_Unit;// Enum.GetName(typeof(Domain.AggregatesModel.MatterAggregate.TimeUnit), EstimatedTimeRequired_Unit);
                return $"{IntervalPeriod_Num}{unit}(s)";
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; private set; }

        /// <summary>
        /// 所属小组，此属性值为null时表示归属个人
        /// </summary>
        public int? TeamId { get; private set; }

        /// <summary>
        /// 所属小组
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 创建者名字
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

    }

    /// <summary>
    /// 事项
    /// </summary>
    public class MatterOutline
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// 重要性
        /// </summary>
        public int Importance { get; private set; }

        /// <summary>
        /// 最后期限
        /// </summary>
        public DateTime? Deadline { get; private set; }

        /// <summary>
        /// 已完成
        /// </summary>
        public bool Finished { get; private set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishTime { get; private set; }

        /// <summary>
        /// 所属小组
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 创建者名字
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

    }

    /// <summary>
    /// 未完成事项概略
    /// </summary>
    public class UnfinishedMatterOutline
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// 重要性
        /// </summary>
        public int Importance { get; private set; }

        /// <summary>
        /// 最后期限
        /// </summary>
        public DateTime? Deadline { get; private set; }

        /// <summary>
        /// 所属小组
        /// </summary>
        public string TeamName { get; set; }

    }

    /*public struct Importance
    {
        public int Value;

        public string Label;
    }

    public struct TimeUnit
    {
        public int Value;

        public string Label;
    }*/

}
