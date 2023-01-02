using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record MakeModel : BaseNopEntityModel
    { 
        #region Properties

        [NopResourceDisplayName("Admin.Make.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Make.Fields.CretedOn")]
        public string CretedOn { get; set; }

        #endregion
    }
}