using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record PartGroupModel : BaseNopEntityModel
    { 
        #region Properties

        [NopResourceDisplayName("Admin.PartGroup.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.PartGroup.Fields.EngineCode")]
        public string EngineCode { get; set; }

        [NopResourceDisplayName("Admin.PartGroup.Fields.CretedOn")]
        public string CretedOn { get; set; }

        #endregion
    }
}