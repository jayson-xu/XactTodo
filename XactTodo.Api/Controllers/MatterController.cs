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
using XactTodo.Domain.AggregatesModel.MatterAggregate;

namespace XactTodo.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MatterController : ControllerBase
    {
        private const string KEY_AUTHORIZATION = "authorization";
        private readonly ICustomSession session;
        private readonly IMatterRepository matterRepository;
        private readonly IMatterQueries matterQueries;

        public MatterController(IMatterRepository matterRepository, IMatterQueries matterQueries, ICustomSession session)
        {
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
        [ProducesResponseType(typeof(IEnumerable<KeyValuePair<int,string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetImportances()
        {
            var kvs = GetEnumKeyValues<Importance>();
            return Ok(kvs);
        }

        [Route("timeunits")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<KeyValuePair<int,string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTimeUnits()
        {
            var kvs = GetEnumKeyValues<TimeUnit>();
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
                var matter = await matterRepository.GetAsync(id);

                return Ok(matter);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        //POST api/v1/[controller]/[action]
        //[Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(MatterInput input)
        {
            var matter = new Domain.AggregatesModel.MatterAggregate.Matter
            {
                Subject = input.Subject,
                Content = input.Content,
                ExecutantId = input.ExecutantId,
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
            if(input.EstimatedTimeRequired!=null && input.EstimatedTimeRequired.Num > 0)
            {
                matter.EstimatedTimeRequired = input.EstimatedTimeRequired;
            }
            if(input.Periodic && input.IntervalPeriod!=null && input.IntervalPeriod.Num>0)
            {
                matter.IntervalPeriod = input.IntervalPeriod;
            }
            matterRepository.Add(matter);
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = matter.Id }, matter.Id);
        }

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
            matter.Password = input.Password;
            matter.RelatedMatterId = input.RelatedMatterId;
            matter.Importance = input.Importance;
            matter.Deadline = input.Deadline;
            matter.Finished = input.Finished;
            matter.FinishTime = input.FinishTime;
            matter.Periodic = input.Periodic;
            matter.Remark = input.Remark;
            matter.TeamId = input.TeamId;
            if (input.EstimatedTimeRequired != null && input.EstimatedTimeRequired.Num > 0)
            {
                matter.EstimatedTimeRequired = input.EstimatedTimeRequired;
            }
            else
            {
                matter.EstimatedTimeRequired = null;
            }
            if (input.Periodic && input.IntervalPeriod != null && input.IntervalPeriod.Num > 0)
            {
                matter.IntervalPeriod = input.IntervalPeriod;
            }
            else
            {
                matter.IntervalPeriod = null;
            }
            matterRepository.Update(matter);
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        [Route("{id:int}/[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Finish(int id)
        {
            return await Finish(id, true);
        }

        [Route("{id:int}/[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Unfinish(int id)
        {
            return await Finish(id, false);
        }

        private async Task<IActionResult> Finish(int id, bool finished)
        {
            var matter = await matterRepository.GetAsync(id);
            if (matter == null)
            {
                return BadRequest($"Matter not found: {id}");
            }
            if (!matter.SetFinished(finished))
                return BadRequest();
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

    }

}