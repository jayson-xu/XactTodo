using System;

namespace XactTodo.Domain.SeedWork
{
    /// <summary>
    /// 该接口由想要存储修改信息的实体实现
    /// </summary>
    public interface IModificationAudited
    {
        /// <summary>
        /// 最后修改人Id
        /// </summary>
        int? LastModifierUserId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }
}
