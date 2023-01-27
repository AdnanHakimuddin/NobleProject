using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record CategoryNavigationModel : BaseNopModel
    {
        public CategoryNavigationModel()
        {
            Categories = new List<CategorySimpleModel>();
        }

        public int CurrentCategoryId { get; set; }
        public List<CategorySimpleModel> Categories { get; set; }
        #region Nested classes

        public record CategoryLineModel : BaseNopModel
        {
            public CategoryLineModel()
            {
                PartGroups = new List<CategorySimpleModel.PartGroupModel>();
            }
            public int CurrentCategoryId { get; set; }
            public int CurrentPartGroupId { get; set; }
            public bool ActivePartGroups { get; set; }
            public CategorySimpleModel Category { get; set; }
            public CategorySimpleModel.PartGroupModel PartGroup { get; set; }
            public List<CategorySimpleModel.PartGroupModel> PartGroups { get; set; }

        }

        #endregion
    }
}