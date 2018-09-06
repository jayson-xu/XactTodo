using System;

namespace XactTodo.Domain.SeedWork
{
    /// <summary>
    /// 该接口由想要存储删除信息的实体实现(何时何人删除)
    /// </summary>
    public interface IDeletionAudited : ISoftDelete
    {
        /// <summary>
        /// 删除人Id
        /// </summary>
        int? DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }
}
