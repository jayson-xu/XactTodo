using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using Microsoft.Extensions.Configuration;
using XactTodo.Security.Session;

namespace XactTodo.Api.Queries
{
    public class MatterQueries : IMatterQueries
    {
        private readonly string connectionString = string.Empty;
        private readonly IClaimsSession session;

        public MatterQueries(IConfiguration configuration, IClaimsSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.connectionString = configuration.GetConnectionString("DefaultConnection")?? throw new Exception("miss configuration item: [DefaultConnection]");
        }

        public async Task<Matter> GetAsync(int id)
        {
            const string sql = @"SELECT M.Id, `Subject`, Content, M.`Password`, ExecutantId, CameFrom, RelatedMatterId, Importance, Status, Deadline, FinishTime, Remark, TeamId, 
M.EstimatedTimeRequired_Num, M.EstimatedTimeRequired_Unit, M.Periodic, M.IntervalPeriod_Num, M.IntervalPeriod_Unit, M.CreationTime, M.CreatorUserId, U.DisplayName Creator
  FROM Matter M
  JOIN `User` U ON M.CreatorUserId=U.Id
  WHERE M.IsDeleted=0 AND M.Id=@id";
            using (var connection = new MySqlConnection(connectionString))
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
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<UnfinishedMatterOutline>(
                    @"SELECT M.Id, M.`Subject`, M.Importance, M.Deadline, T.`Name` as TeamName
                     FROM Matter M
                     LEFT JOIN Team T ON M.TeamId = T.Id
                     WHERE M.Status>=0 and M.Status <100 and (M.CreatorUserId=@userId OR M.ExecutantId=@userId)
                     ORDER BY M.Importance DESC",
                    new { userId }
                    );
            }
        }

        /// <summary>
        /// 查询全部事项
        /// </summary>
        /// <param name="search">搜索关键字</param>
        /// <param name="status">进展情况</param>
        /// <param name="page">第几页，从1开始</param>
        /// <param name="limit">每页限定行数</param>
        /// <param name="sortOrder">排序字段，格式如：column1[, column2 [DESC]]</param>
        /// <returns></returns>
        public async Task<PaginatedData<MatterOutline>> GetMattersAsync(string search, ProgressStatus? status, int page, int limit, string sortOrder)
        {
            var sql = @"SELECT M.Id, M.`Subject`, M.Content, M.Importance, M.Status, M.Deadline, M.StartTime, M.FinishTime, T.`Name` as TeamName, M.CreationTime, U.DisplayName CreatorName
  FROM Matter M
  LEFT JOIN Team T ON M.TeamId = T.Id
  LEFT JOIN User U ON M.CreatorUserId=U.Id
  WHERE (M.CreatorUserId=@userId OR M.ExecutantId=@userId)";
            if (status.HasValue)
            {
                sql += " AND M.Status=@status";
            }
            if (!string.IsNullOrEmpty(search))
            {
                search = $"%{search}%";
                sql += " AND (M.Subject Like @search OR M.Content Like @search)";
            }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var param = new { session.UserId, status, search };
                var matters = await PaginatedData<MatterOutline>.ExecuteAsync(connection, sql, param, sortOrder, page, limit);
                return matters;
            }
        }
    }
}
