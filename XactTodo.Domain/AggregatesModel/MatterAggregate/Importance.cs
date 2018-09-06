namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{

    /// <summary>
    /// 重要性枚举
    /// </summary>
    public enum Importance
    {
        /// <summary>
        /// 不确定
        /// </summary>
        Uncertain = 0,

        /// <summary>
        /// 不重要
        /// </summary>
        Unimportant = 1,

        /// <summary>
        /// 一般
        /// </summary>
        Normal = 2,

        /// <summary>
        /// 重要
        /// </summary>
        Important = 3,

        /// <summary>
        /// 非常重要
        /// </summary>
        VeryImportant = 4,

    }
}