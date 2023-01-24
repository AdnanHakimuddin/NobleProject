using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public record CategorySimpleModel : BaseNopEntityModel
    {
        public CategorySimpleModel()
        {
            SubCategories = new List<CategorySimpleModel>();
            PartGroups = new List<PartGroupModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int? NumberOfProducts { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public List<PartGroupModel> PartGroups { get; set; }
        public List<CategorySimpleModel> SubCategories { get; set; }

        public bool HaveSubCategories { get; set; }

        public string Route { get; set; }

        public partial record PartGroupModel : BaseNopEntityModel
        {
            public PartGroupModel()
            {
                PartTypes = new List<PartTypeModel>();
            }
            public List<PartTypeModel> PartTypes { get; set; }
            public string Name { get; set; }

            public string SeName { get; set; }

            public partial record PartTypeModel : BaseNopEntityModel
            {
                public string Name { get; set; }
            }
        }

    }
}