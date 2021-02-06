using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.MatterAggregate;

namespace XactTodo.Api.Queries
{
    /// <summary>
    /// 事项查询接口
    /// </summary>
    public interface IMatterQueries : IQueries
    {
        Task<Matter> GetAsync(int id);

        Task<IEnumerable<UnfinishedMatterOutline>> GetUnfinishedMatterAsync(int userId);

        Task<PaginatedData<MatterOutline>> GetMattersAsync(string search, ProgressStatus? status, int page, int limit, string sortOrder);

        //Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}
