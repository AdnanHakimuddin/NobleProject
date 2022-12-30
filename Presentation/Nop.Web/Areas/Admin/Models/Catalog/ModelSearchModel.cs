using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record ModelSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Model.List.SearchYearName")]
        public string SearchName { get; set; }
       
        #endregion
    }
}