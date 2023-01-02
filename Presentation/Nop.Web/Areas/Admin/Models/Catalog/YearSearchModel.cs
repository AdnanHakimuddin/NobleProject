using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record YearSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Year.List.SearchYearName")]
        public string SearchName { get; set; }
       
        #endregion
    }
}