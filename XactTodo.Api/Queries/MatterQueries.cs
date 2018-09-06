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

        public async Task<Matter> GetMatterAsync(int id)
        {
            const string sql = @"";
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
