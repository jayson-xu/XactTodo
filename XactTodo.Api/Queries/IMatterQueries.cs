using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Queries
{
    /// <summary>
    /// 事项查询接口
    /// </summary>
    public interface IMatterQueries : IQueries
    {
        Task<Matter> GetAsync(int id);

        Task<IEnumerable<UnfinishedMatterOutline>> GetUnfinishedMatterAsync(int userId);

        //Task<IEnumerable<MatterSummary>> GetMattersAsync();

        //Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}
