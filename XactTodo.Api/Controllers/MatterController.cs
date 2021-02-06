using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XactTodo.Api.DTO;
using XactTodo.Api.Queries;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Security.Session;
using XactTodo.Api.Utils;

namespace XactTodo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatterController : ControllerBase
    {
        private const string KEY_AUTHORIZATION = "authorization";
        private readonly IClaimsSession session;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger logger;
        private readonly IMatterRepository matterRepository;
        private readonly IMatterQueries matterQueries;
        private static Dictionary<int, string> timeUnits;
        private static Dictionary<int, string> importances;
        private static Dictionary<int, string> progressStatuses;

        public MatterController(
            IHttpContextAccessor httpContextAccessor,
            ILogger<MatterController> logger,
            IMatterRepository matterRepository,
            IMatterQueries matterQueries,
            IClaimsSession session)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.matterRepository = matterRepository ?? throw new ArgumentNullException(nameof(matterRepository));
            this.matterQueries = matterQueries ?? throw new ArgumentNullException(nameof(matterQueries));
            InitializeStaticData();
        }

        private void InitializeStaticData()
        {
            if (timeUnits == null)
            {
                timeUnits = new Dictionary<int, string>
                {
                    { (int)TimeUnit.Weekday, "工作日" },
                    { (int)TimeUnit.NaturalDay, "自然日" },
                    { (int)TimeUnit.Week, "周" },
                    { (int)TimeUnit.Month, "月" },
                    { (int)TimeUnit.Year, "年" },
                };
            }
            if (importances == null)
            {
                importances = new Dictionary<int, string>
                {
                    { (int)Importance.Uncertain, "不确定"},
                    { (int)Importance.Unimportant, "不重要"},
                    { (int)Importance.Normal, "一般"},
                    { (int)Importance.Important, "重要"},
                    { (int)Importance.VeryImportant, "非常重要"},
                };
            }
            if(progressStatuses==null)
            {
                progressStatuses = new Dictionary<int, string>
                {
                    {(int)ProgressStatus.NotStarted, "未开始" },
                    {(int)ProgressStatus.Suspend, "暂停" },
                    {(int)ProgressStatus.InProgress, "进行中" },
                    {(int)ProgressStatus.Finished, "已完成" },
                    {(int)ProgressStatus.Aborted, "中止" },
                };
            }
        }

        private static IEnumerable<KeyValuePair<int,string>> GetEnumKeyValues<T>() where T: Enum
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            var kvs = new KeyValuePair<int, string>[values.Length];
            for(int i=0; i<values.Length; i++)
            {
                kvs[i] = new KeyValuePair<int, string>((int)(object)values[i], values[i].ToString());
            }
            return kvs;
        }

        /// <summary>
        /// 获取重要性列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("importances")]
        [ProducesResponseType(typeof(IEnumerable<ValueTextPair<int>>), (int)HttpStatusCode.OK)]
        public IActionResult GetImportances()
        {
            // 将字典转换为ValueTextPair数组，否则前台得到的将是一个对象：{"0":"不确定", "1": "不重要"...}
            return Ok(importances.ToValueTextCollection());
        }

        /// <summary>
        /// 获取时间单位列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("timeunits")]
        [ProducesResponseType(typeof(IEnumerable<ValueTextPair<int>>), (int)HttpStatusCode.OK)]
        public IActionResult GetTimeUnits()
        {
            // 将字典转换为ValueTextPair数组，否则前台得到的将是一个对象：{"0":"工作日", "1": "自然日"...}
            return Ok(timeUnits.ToValueTextCollection());
        }

        /// <summary>
        /// 获取进展情况集合
        /// </summary>
        /// <returns></returns>
        [HttpGet("progressstatuses")]
        [ProducesResponseType(typeof(IEnumerable<ValueTextPair<int>>), (int)HttpStatusCode.OK)]
        public IActionResult GetProgressStatuses()
        {
            return Ok(progressStatuses.ToValueTextCollection());
        }

        /// <summary>
        /// 获取未完成的事项
        /// </summary>
        /// <param name="excludedTeamsId">排除指定小组(例如私密)</param>
        /// <returns></returns>
        [HttpGet("unfinished")]
        [ProducesResponseType(typeof(IEnumerable<UnfinishedMatterOutline>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUnfinishedMatters(string excludedTeamsId)
        {
            var au = Request.Headers[KEY_AUTHORIZATION];
            session.VerifyLoggedin();
            var unfinishedMatters = await matterQueries.GetUnfinishedMatterAsync(session.UserId.Value);
            return Ok(unfinishedMatters);
        }

        /// <summary>
        /// 获取指定事项
        /// </summary>
        /// <param name="id">事项Id</param>
        /// <returns></returns>
        [HttpGet("{id:int}")] //加上类型声明的好处是，如果传入的参数不是整数则直接返回404，不加则返回400并报告错误"The value 'xxx' is not valid."
        [ProducesResponseType(typeof(Queries.Matter), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            try
            {
                var matter = await matterQueries.GetAsync(id);

                return Ok(matter);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 获取全部事项
        /// </summary>
        /// <param name="search"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(string search, ProgressStatus? status, int page = 1, int limit = 10, string sortOrder = "")
        {
            var matters = await matterQueries.GetMattersAsync(search, status, page, limit, sortOrder);
            foreach(var matter in matters.Rows)
            {
                matter.ProgressStatus = progressStatuses[matter.Status];
            }
            return Ok(matters);
        }

        /// <summary>
        /// 创建新事项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //POST api/v1/[controller]/[action]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(MatterInput input)
        {
            var matter = new Domain.AggregatesModel.MatterAggregate.Matter
            {
                Subject = input.Subject,
                Content = input.Content,
                ExecutantId = input.ExecutantId,
                CameFrom = input.CameFrom,
                Password = input.Password,
                RelatedMatterId = input.RelatedMatterId,
                Importance = input.Importance,
                Deadline = input.Deadline,
                Status = ProgressStatus.NotStarted,
                Periodic = input.Periodic,
                Remark = input.Remark,
                TeamId = input.TeamId
            };
            if(input.EstimatedTimeRequired_Num > 0)
            {
                matter.EstimatedTimeRequired = new PeriodOfTime(input.EstimatedTimeRequired_Num, input.EstimatedTimeRequired_Unit);
            }
            if(input.Periodic && input.IntervalPeriod_Num>0)
            {
                matter.IntervalPeriod = new PeriodOfTime(input.IntervalPeriod_Num, input.IntervalPeriod_Unit);
            }
            matterRepository.Add(matter);
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = matter.Id }, matter.Id);
        }

        /// <summary>
        /// 更新事项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        //PUT api/v1/[controller]/{id}
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update([FromRoute] int id, MatterInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != input.Id)
            {
                return BadRequest();
            }
            var matter = await matterRepository.GetAsync(id);
            matter.Subject = input.Subject;
            matter.Content = input.Content;
            matter.ExecutantId = input.ExecutantId;
            matter.CameFrom = input.CameFrom;
            matter.Password = input.Password;
            matter.RelatedMatterId = input.RelatedMatterId;
            matter.Importance = input.Importance;
            matter.Deadline = input.Deadline;
            matter.StartTime = input.StartTime;
            matter.FinishTime = input.FinishTime;
            matter.Periodic = input.Periodic;
            matter.Remark = input.Remark;
            matter.TeamId = input.TeamId;
            if (input.EstimatedTimeRequired_Num > 0)
            {
                matter.EstimatedTimeRequired = new PeriodOfTime(input.EstimatedTimeRequired_Num, input.EstimatedTimeRequired_Unit);
            }
            else
            {
                matter.EstimatedTimeRequired = null;
            }
            if (input.Periodic && input.IntervalPeriod_Num > 0)
            {
                matter.IntervalPeriod = new PeriodOfTime(input.IntervalPeriod_Num, input.IntervalPeriod_Unit);
            }
            else
            {
                matter.IntervalPeriod = null;
            }
            matterRepository.Update(matter);
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 删除事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            try
            {
                matterRepository.Delete(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        /*
        /// <summary>
        /// 开始(或恢复)执行指定事项
        /// </summary>
        [HttpPost("{id:int}/[action]")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Start(int id, CommentInput input)
        {
            return await UpdateProgress(id, ProgressStatus.InProgress, input.Comment, input.StartTime, null);
        }

        /// <summary>
        /// 暂停执行指定事项
        /// </summary>
        [HttpPost("{id:int}/[action]")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Suspend(int id, CommentInput input)
        {
            return await UpdateProgress(id, ProgressStatus.Suspend, input.Comment, null, null);
        }

        /// <summary>
        /// 将指定事项更新为已完成状态
        /// </summary>
        [HttpPost("{id:int}/[action]")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Finish(int id, CommentInput input)
        {
            return await UpdateProgress(id, ProgressStatus.Finished, input.Comment, input.StartTime, input.FinishTime);
        }

        /// <summary>
        /// 中止执行指定事项
        /// </summary>
        [HttpPost("{id:int}/[action]")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Abort(int id, CommentInput input)
        {
            return await UpdateProgress(id, ProgressStatus.Aborted, input.Comment, null, null);
        }
        */

        /// <summary>
        /// 更新事项进展状况
        /// </summary>
        [HttpPost("{id:int}/[action]")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProgressStatus(int id, ProgressUpdateAsk ask)
        {
            if (!Enum.IsDefined(typeof(ProgressStatus), ask.NewStatus))
                return BadRequest($"无效的进展状况值：{ask.NewStatus}");
            return await UpdateProgress(id, (ProgressStatus)ask.NewStatus, ask.Comment, ask.StartTime, ask.FinishTime);
        }

        private async Task<IActionResult> UpdateProgress(int id, ProgressStatus status, string comment, DateTime?startTime, DateTime? finishTime)
        {
            var matter = await matterRepository.GetAsync(id);
            if (matter == null)
            {
                return BadRequest($"Matter not found: {id}");
            }
            matter.UpdateProgress(session, status, comment, startTime, finishTime);
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

    }

}