using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Queries
{
    public interface IMatterQueries
    {
        Task<Matter> GetMatterAsync(int id);

        Task<IEnumerable<UnfinishedMatterOutline>> GetUnfinishedMatterAsync(int userId);

        //Task<IEnumerable<MatterSummary>> GetMattersAsync();

        //Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}
