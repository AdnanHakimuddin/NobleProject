using System;

namespace Nop.Core.Domain.Catalog
{
    public partial class Model : BaseEntity
    {
        public string Name { get; set; }
        public int ModelId { get; set; }
        public int YearId { get; set; }
        public int MakeId { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
