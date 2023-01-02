using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record MakeSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Make.List.SearchYearName")]
        public string SearchName { get; set; }
       
        #endregion
    }
}