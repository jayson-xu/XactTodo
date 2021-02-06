namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{

    /// <summary>
    /// 进展状态枚举
    /// </summary>
    public enum ProgressStatus
    {

        /// <summary>
        /// 中止
        /// </summary>
        Aborted = -1,

        /// <summary>
        /// 未开始
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// 暂停
        /// </summary>
        Suspend = 1,

        /// <summary>
        /// 进行中
        /// </summary>
        InProgress = 10,

        /// <summary>
        /// 已完成
        /// </summary>
        Finished = 100,

    }
}