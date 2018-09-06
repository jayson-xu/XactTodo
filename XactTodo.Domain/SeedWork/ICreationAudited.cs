using System;

namespace XactTodo.Domain.SeedWork
{
    /// <summary>
    /// 该接口由想要存储创建信息的实体实现
    /// </summary>
    public interface ICreationAudited
    {
        /// <summary>
        /// 创建者Id
        /// </summary>
        int CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreationTime { get; set; }

    }
}
