using System;

namespace Nop.Core.Domain.Catalog
{
    public partial class PartGroup : BaseEntity
    {
        public string Name { get; set; }
        public string EngineCode { get; set; }
        public int PartTypeId { get; set; }
        public string ApiPartGroupId { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
