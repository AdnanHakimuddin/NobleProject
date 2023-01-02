using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial record ModelModel : BaseNopEntityModel
    { 
        #region Properties

        [NopResourceDisplayName("Admin.Model.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Model.Fields.CretedOn")]
        public string CretedOn { get; set; }

        #endregion
    }
}