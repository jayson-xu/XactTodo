using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XactTodo.Api.DTO;
using XactTodo.Api.Queries;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.MatterAggregate;

namespace XactTodo.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MatterController : ControllerBase
    {
        private const string KEY_AUTHORIZATION = "authorization";
        private readonly ICustomSession session;
        private readonly ILogger logger;
        private readonly IMatterRepository matterRepository;
        private readonly IMatterQueries matterQueries;

        public MatterController(
            ILogger<MatterController> logger,
            IMatterRepository matterRepository,
            IMatterQueries matterQueries,
            ICustomSession session)
        {
            this.logger = logger;
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.matterRepository = matterRepository ?? throw new ArgumentNullException(nameof(matterRepository));
            this.matterQueries = matterQueries ?? throw new ArgumentNullException(nameof(matterQueries)); ;
        }

        private IEnumerable<KeyValuePair<int,string>> GetEnumKeyValues<T>() where T: Enum
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            var kvs = new KeyValuePair<int, string>[values.Length];
            for(int i=0; i<values.Length; i++)
            {
                kvs[i] = new KeyValuePair<int, string>((int)(object)values[i], values[i].ToString());
            }
            return kvs;
        }

        [Route("importances")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<KeyValuePair<int, string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetImportances()
        {
            //var kvs = GetEnumKeyValues<Importance>();
            var kvs = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>((int)Importance.Uncertain, "不确定"),
                new KeyValuePair<int, string>((int)Importance.Unimportant, "不重要"),
                new KeyValuePair<int, string>((int)Importance.Normal, "一般"),
                new KeyValuePair<int, string>((int)Importance.Important, "重要"),
                new KeyValuePair<int, string>((int)Importance.VeryImportant, "非常重要"),
            };
            return Ok(kvs);
        }

        [Route("timeunits")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<KeyValuePair<int,string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTimeUnits()
        {
            var kvs = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>((int)TimeUnit.Weekday, "工作日"),
                new KeyValuePair<int, string>((int)TimeUnit.NaturalDay, "自然日"),
                new KeyValuePair<int, string>((int)TimeUnit.Week, "周"),
                new KeyValuePair<int, string>((int)TimeUnit.Month, "月"),
                new KeyValuePair<int, string>((int)TimeUnit.Year, "年"),
            };
            return Ok(kvs);
        }

        [Route("unfinished")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UnfinishedMatterOutline>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUnfinishedMatters(string excludedTeamsId)
        {
            var au = Request.Headers[KEY_AUTHORIZATION];
            session.VerifyLoggedin();
            var unfinishedMatters = await matterQueries.GetUnfinishedMatterAsync(session.UserId.Value);
            return Ok(unfinishedMatters);
        }

        [Route("{id:int}")] //加上类型声明的好处是，如果传入的参数不是整数则直接返回404，不加则返回400并报告错误"The value 'xxx' is not valid."
        [HttpGet]
        [ProducesResponseType(typeof(Queries.Matter), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetById(int id)
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
        /// 创建新事项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //POST api/v1/[controller]/[action]
        //[Route("[action]")]
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
                Finished = input.Finished,
                FinishTime = input.FinishTime,
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
            matter.Finished = input.Finished;
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
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id)
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

        [Route("{id:int}/[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Finish(int id, string comment)
        {
            return await Finish(id, true, comment);
        }

        [Route("{id:int}/[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Unfinish(int id, string comment)
        {
            return await Finish(id, false, comment);
        }

        private async Task<IActionResult> Finish(int id, bool finished, string comment)
        {
            var matter = await matterRepository.GetAsync(id);
            if (matter == null)
            {
                return BadRequest($"Matter not found: {id}");
            }
            if (!matter.SetFinished(finished, comment, session))
                return BadRequest();
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

    }

}