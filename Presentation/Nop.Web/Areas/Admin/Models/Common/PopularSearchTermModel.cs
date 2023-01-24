using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a popular search term model
    /// </summary>
    public partial record PopularSearchTermModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.SearchTermReport.Keyword")]
        public string Keyword { get; set; }

        [NopResourceDisplayName("Admin.SearchTermReport.Count")]
        public int Count { get; set; }
        
        [NopResourceDisplayName("Admin.SearchTermReport.Year")]
        public string Year { get; set; }
        
        [NopResourceDisplayName("Admin.SearchTermReport.Make")]
        public string Make { get; set; }
        
        [NopResourceDisplayName("Admin.SearchTermReport.Model")]
        public string Model { get; set; }
        
        [NopResourceDisplayName("Admin.SearchTermReport.Engine")]
        public string Engine { get; set; }

        #endregion
    }
}
