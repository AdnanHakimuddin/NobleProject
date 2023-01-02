using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record YearModel : BaseNopEntityModel
    { 
        #region Properties

        [NopResourceDisplayName("Admin.Year.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Year.Fields.CretedOn")]
        public string CretedOn { get; set; }

        #endregion
    }
}