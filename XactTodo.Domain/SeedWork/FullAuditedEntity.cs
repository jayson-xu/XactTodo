using System;
using System.Collections.Generic;
using System.Text;

namespace XactTodo.Domain.SeedWork
{
    public abstract class FullAuditedEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>, ICreationAudited, IModificationAudited, IDeletionAudited//, ISoftDelete
    {
        public virtual int CreatorUserId { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual int? LastModifierUserId { get; set; }

        public virtual DateTime? LastModificationTime { get; set; }

        public virtual int? DeleterUserId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }

    }

    public class FullAuditedEntity : FullAuditedEntity<int>
    {
    }
}
