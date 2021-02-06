using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.MatterAggregate;

namespace XactTodo.Api.DTO
{
    /// <summary>
    /// 更新事项进展状况的请求
    /// </summary>
    public class ProgressUpdateAsk
    {
        /// <summary>
        /// 最新进展状况<seealso cref="ProgressStatus"/>
        /// </summary>
        public int NewStatus { get; set; }

        /// <summary>
        /// 进展说明
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 事项开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 事项完成时间
        /// </summary>
        public DateTime? FinishTime { get; set; }

    }
}
