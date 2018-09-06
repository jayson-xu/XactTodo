using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{
    public class Matter : FullAuditedEntity, IAggregateRoot
    {
        private const int MaxPasswordLength = 128;
        public const int MaxSubjectLength = 200;
        public const int MaxRemarkLength = 500;

        [Required]
        [StringLength(MaxSubjectLength)]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Content { get; set; }

        /// <summary>
        /// 负责人Id
        /// </summary>
        public int? ExecutantId { get; set; }

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

        public bool Finished { get; set; }

        public DateTime? FinishTime { get; set; }

        public bool Periodic { get; set; }

        public PeriodOfTime IntervalPeriod {get; set;}

        [StringLength(MaxRemarkLength)]
        public string Remark { get; set; }

        /// <summary>
        /// 所属小组，此属性值为null时表示归属个人
        /// </summary>
        public int? TeamId { get; set; }

        public ICollection<Evolvement> Evolvements { get; set; }

        //public IEnumerable<Attachment> Attachments { get; set; }

        public bool SetFinished(bool finished)
        {
            if (this.Finished == finished)
                return false;
            this.Finished = finished;
            this.FinishTime = finished ? DateTime.Now : (DateTime?)null;
            return true;
        }
    }
}
