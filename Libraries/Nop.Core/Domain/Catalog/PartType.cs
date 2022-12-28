using System;

namespace Nop.Core.Domain.Catalog
{
    public partial class PartType : BaseEntity
    {
        public string Name { get; set; }
        public int GroupId { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
