using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record EngineSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Engine.List.SearcName")]
        public string SearchName { get; set; }
       
        #endregion
    }
}