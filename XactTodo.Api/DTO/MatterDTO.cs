using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.MatterAggregate;

namespace XactTodo.Api.DTO
{
    /// <summary>
    /// 事项
    /// </summary>
    public class MatterInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [StringLength(Matter.MaxSubjectLength)]
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 负责人Id
        /// </summary>
        public int? ExecutantId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <remarks>如果设定了此密码，则在查看或编辑事项详情时必须先核对密码，事项创建人可重置此密码</remarks>
        [StringLength(20)]
        public string Password { get; set; }

        /// <summary>
        /// 关联事项
        /// </summary>
        public int? RelatedMatterId { get; set; }

        /// <summary>
        /// 重要性
        /// </summary>
        public Importance Importance { get; set; }

        /// <summary>
        /// 预计需时
        /// </summary>
        public PeriodOfTime EstimatedTimeRequired { get; set; }

        /// <summary>
        /// 最后期限
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// 已完成
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// 周期性事项
        /// </summary>
        public bool Periodic { get; set; }

        /// <summary>
        /// 间隔周期
        /// </summary>
        public PeriodOfTime IntervalPeriod { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(Matter.MaxRemarkLength)]
        public string Remark { get; set; }

        /// <summary>
        /// 所属小组，此属性值为null时表示归属个人
        /// </summary>
        public int? TeamId { get; set; }
    }


}
