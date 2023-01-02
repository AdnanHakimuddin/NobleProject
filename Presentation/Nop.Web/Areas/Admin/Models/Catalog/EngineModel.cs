using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record EngineModel : BaseNopEntityModel
    { 
        #region Properties

        [NopResourceDisplayName("Admin.Engine.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Engine.Fields.CretedOn")]
        public string CretedOn { get; set; }

        #endregion
    }
}