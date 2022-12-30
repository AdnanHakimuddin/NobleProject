using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record PartGroupSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.PartGroup.List.SearchYearName")]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Admin.PartGroup.List.SearchPartTypeName")]
        public string SearchPartTypeName { get; set; }

        #endregion
    }
}