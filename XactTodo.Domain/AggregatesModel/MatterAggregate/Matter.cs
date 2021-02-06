using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XactTodo.Domain.SeedWork;
using XactTodo.Security;
using XactTodo.Security.Session;

namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{
    public class Matter : FullAuditedEntity, IAggregateRoot
    {
        private const int MaxPasswordLength = 128;
        public const int MaxSubjectLength = 200;
        public const int MaxRemarkLength = 500;
        public const int MaxCameFromLength = 50;

        /// <summary>
        /// 主题
        /// </summary>
        [Required]
        [StringLength(MaxSubjectLength)]
        public string Subject { get; set; }

        /// <summary>
        /// 具体内容
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string Content { get; set; }

        /// <summary>
        /// 负责人Id
        /// </summary>
        public int? ExecutantId { get; set; }

        /// <summary>
        /// 事项来源
        /// </summary>
        [StringLength(MaxCameFromLength)]
        public string CameFrom { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <remarks>如果设定了此密码，则在查看或编辑事项详情时必须先核对密码，事项创建人可重置此密码</remarks>
        [StringLength(MaxPasswordLength)]
        public string Password { get; set; }

        /// <summary>
        /// 关联事项
        /// </summary>
        public int? RelatedMatterId { get; set; }

        public Importance Importance { get; set; }

        public PeriodOfTime EstimatedTimeRequired { get; set; }

        public DateTime? Deadline { get; set; }

        public ProgressStatus Status { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? FinishTime { get; set; }

        public bool Periodic { get; set; }

        public PeriodOfTime IntervalPeriod { get; set; }

        [StringLength(MaxRemarkLength)]
        public string Remark { get; set; }

        /// <summary>
        /// 所属小组，此属性值为null时表示归属个人
        /// </summary>
        public int? TeamId { get; set; }

        public ICollection<Evolvement> Evolvements { get; set; } = new List<Evolvement>();

        //public IEnumerable<Attachment> Attachments { get; set; }

        public void UpdateProgress(IClaimsSession session, ProgressStatus status, string comment, DateTime? startTime=null, DateTime? finishTime=null)
        {
            if (this.Status == status)
                throw new ApplicationException($"该事项当前进展状况与欲更新为进展状况相同，可能后台数据已更新");
            string descr;
            this.FinishTime = null;
            switch (status)
            {
                case ProgressStatus.NotStarted:
                    descr = "暂不安排";
                    this.StartTime = null;
                    break;
                case ProgressStatus.Suspend:
                    descr = "暂缓计划";
                    break;
                case ProgressStatus.InProgress:
                    //如果之前不是暂停状态，则重新计时
                    if (this.Status != ProgressStatus.Suspend)
                    {
                        descr = "重启事项";
                        this.StartTime = startTime ?? DateTime.Now;
                    }
                    else
                    {
                        descr = "恢复执行";
                    }
                    break;
                case ProgressStatus.Finished:
                    if (startTime.HasValue)
                        this.StartTime = startTime.Value;
                    this.FinishTime = finishTime?? DateTime.Now;
                    descr = "事项完成";
                    break;
                case ProgressStatus.Aborted:
                    descr = "事项中止";
                    break;
                default:
                    throw new ApplicationException($"未处理的枚举值：{status}");
            }
            this.Status = status;
            this.Evolvements.Add(new Evolvement
            {
                Comment = descr + (string.IsNullOrWhiteSpace(comment) ? "" : "：")
                + $"{comment} by {session.NickName}({session.UserName})",
                MatterId = this.Id,
            });
        }
    }
}
