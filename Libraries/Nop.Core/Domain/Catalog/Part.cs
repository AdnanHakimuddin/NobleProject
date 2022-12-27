using System;

namespace Nop.Core.Domain.Catalog
{
    public partial class Part : BaseEntity
    {
        public string Name { get; set; }
        public int PartGroupId { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
