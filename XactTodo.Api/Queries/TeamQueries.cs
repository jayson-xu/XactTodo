using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using XactTodo.Security.Session;

namespace XactTodo.Api.Queries
{
    public class TeamQueries : ITeamQueries
    {
        private readonly IClaimsSession session;
        private readonly string connectionString;

        public TeamQueries(IConfiguration configuration, IClaimsSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("miss configuration item: [DefaultConnection]");
        }

        public async Task<Team> GetAsync(int id)
        {
            const string sql = @"SELECT Id, Name, ProposedTags FROM Team WHERE IsDeleted=0 AND Id=@id";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var rlt = await connection.QuerySingleOrDefaultAsync(sql, new { id });
                if (rlt == null)
                    throw new KeyNotFoundException();
                var team = new Team
                {
                    Id = rlt.Id,
                    Name = rlt.Name,
                    ProposedTags = string.IsNullOrWhiteSpace(rlt.ProposedTags) ? new string[0] : rlt.ProposedTags.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries),
                };
                team.Members.AddRange(await QueryMembersAsync(team.Id, connection));

                return team;
            }
        }

        private async Task<IEnumerable<MemberOutline>> QueryMembersAsync(int teamId, DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            const string sql = @"SELECT M.Id, M.UserId, U.UserName, U.DisplayName, U.Email, M.IsSupervisor FROM Member M JOIN `User` U ON M.UserId=U.Id WHERE M.IsDeleted=0 AND M.TeamId=@teamId";
            return await connection.QueryAsync<MemberOutline>(sql, new { teamId });
        }

        public async Task<IEnumerable<TeamOutline>> GetJoinedTeamsAsync(int userId, bool incCreatedTeams)
        {
            const string where = "EXISTS(SELECT 1 FROM Member M WHERE M.IsDeleted=0 AND M.TeamId=Team.Id AND M.UserId=@userId)";
            return await QueryTeams(where, new { userId });
        }

        public async Task<IEnumerable<TeamOutline>> GetManagedTeamsAsync(int userId)
        {
            const string where = "EXISTS(SELECT 1 FROM Member M WHERE M.IsDeleted=0 AND M.TeamId=Team.Id AND M.IsSupervisor=1 AND M.UserId=@userId)";
            return await QueryTeams(where, new { userId });
        }

        private async Task<IEnumerable<TeamOutline>> QueryTeams(string whereClause, object param)
        {
            string sql = @"SELECT Id, Name, ProposedTags FROM Team WHERE IsDeleted=0 AND "+whereClause;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var rlts = await connection.QueryAsync(sql, param);
                var teams = new TeamOutline[rlts.Count()];
                var i=0;
                foreach(var rlt in rlts)
                {
                    teams[i++] = new TeamOutline
                    {
                        Id = rlt.Id,
                        Name = rlt.Name,
                        ProposedTags = string.IsNullOrWhiteSpace(rlt.ProposedTags) ? new string[0] : rlt.ProposedTags.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries),
                    };
                }
                return teams;
            }
        }

        public async Task<IEnumerable<MemberOutline>> GetMembersAsync(int teamId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return await QueryMembersAsync(teamId, connection);
            }
        }

    }
}
