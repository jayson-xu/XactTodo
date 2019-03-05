using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;

namespace XactTodo.Api.Queries
{
    public class MatterQueries : IMatterQueries
    {
        private string _connectionString = string.Empty;

        public MatterQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<Matter> GetAsync(int id)
        {
            const string sql = @"SELECT M.Id, `Subject`, Content, M.`Password`, ExecutantId, CameFrom, RelatedMatterId, Importance, Deadline, Finished, FinishTime, Remark, TeamId, 
E.Num EstimatedTimeRequired_Num, E.Unit EstimatedTimeRequired_Unit, M.Periodic, I.Num IntervalPeriod_Num, I.Unit IntervalPeriod_Unit,
M.CreationTime, M.CreatorUserId, U.DisplayName Creator
  FROM Matter M
  LEFT JOIN `Matter.EstimatedTimeRequired#PeriodOfTime` E on E.MatterId=M.Id
  LEFT JOIN `Matter.IntervalPeriod#PeriodOfTime` I on I.MatterId=M.Id
  JOIN `User` U ON M.CreatorUserId=U.Id
  WHERE M.IsDeleted=0 AND M.Id=@id";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var matter = await connection.QuerySingleOrDefaultAsync<Matter>(sql, new { id });
                if (matter == null)
                    throw new KeyNotFoundException();
                await LoadEvolvements(matter, connection);

                return matter;
            }
        }

        private async Task LoadEvolvements(Matter matter, DbConnection connection)
        {
        }

        public async Task<IEnumerable<UnfinishedMatterOutline>> GetUnfinishedMatterAsync(int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<UnfinishedMatterOutline>(
                    @"SELECT M.Id, M.`Subject`, M.Importance, M.Deadline, T.`Name` as TeamName
                     FROM Matter M
                     LEFT JOIN Team T ON M.TeamId = T.Id
                     WHERE M.Finished=0 and (M.CreatorUserId=@userId OR M.ExecutantId=@userId)
                     ORDER BY M.Importance DESC",
                    new { userId }
                    );
            }
        }
    }
}
