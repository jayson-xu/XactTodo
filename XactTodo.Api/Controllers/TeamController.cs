using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XactTodo.Api.DTO;
using XactTodo.Api.Queries;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Security.Session;

namespace XactTodo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IClaimsSession session;
        private readonly ITeamRepository teamRepository;
        private readonly ITeamQueries teamQueries;

        public TeamController(ITeamRepository teamRepository, ITeamQueries teamQueries, IClaimsSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            this.teamQueries = teamQueries ?? throw new ArgumentNullException(nameof(teamQueries));
        }

        /// <summary>
        /// 获取当前用户加入的所有小组
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TeamOutline>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetJoinedTeams()
        {
            session.VerifyLoggedin();
            var teams = await teamQueries.GetJoinedTeamsAsync(session.UserId.Value);
            return Ok(teams);
        }

        /// <summary>
        /// 获取当前用户有管理权的小组
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TeamOutline>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetManagedTeams()
        {
            session.VerifyLoggedin();
            var teams = await teamQueries.GetManagedTeamsAsync(session.UserId.Value);
            return Ok(teams);
        }

        [Route("{id:int}")] //加上类型声明的好处是，如果传入的参数不是整数则直接返回404，不加则返回400并报告错误"The value 'xxx' is not valid."
        [HttpGet]
        [ProducesResponseType(typeof(Queries.Team), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var team = await teamQueries.GetAsync(id);
                return Ok(team);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            /*var team = await teamRepository.GetAsync(id);
            return Ok(
                new Queries.Team
                {
                    Name = team.Name,
                    ProposedTags = string.IsNullOrWhiteSpace(team.ProposedTags) ? new string[0] : team.ProposedTags.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries),
                });*/
        }

        /// <summary>
        /// 创建新小组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //POST api/v1/[controller]/[action]
        //[Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(TeamInput input)
        {
            var team = new Domain.AggregatesModel.TeamAggregate.Team
            {
                Name = input.Name,
                ProposedTags = input.ProposedTags,
                LeaderId = session.UserId.Value,
            };
            team.Members.Add(new Member
            {
                TeamId = team.Id,
                UserId = session.UserId.Value,
                IsSupervisor = true,
            });
            teamRepository.Add(team);
            await teamRepository.UnitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = team.Id }, team.Id);
        }

        /// <summary>
        /// 更新小组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        //PUT api/v1/[controller]/{id}
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update([FromRoute] int id, TeamInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != input.Id)
            {
                return BadRequest();
            }
            var team = await teamRepository.GetAsync(id);
            team.Name = input.Name;
            team.ProposedTags = input.ProposedTags;
            teamRepository.Update(team);
            await teamRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 删除小组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                teamRepository.Delete(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            await teamRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

    }
}