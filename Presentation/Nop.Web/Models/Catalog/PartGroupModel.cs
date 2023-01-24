using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial record PartGroupModel : BaseNopEntityModel
    {
        public PartGroupModel()
        {
            PartTypes = new List<PartTypeModel>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        
        public IList<PartTypeModel> PartTypes { get; set; }

        #region Nested Classes

        public partial record PartTypeModel : BaseNopEntityModel
        {
            public string Name { get; set; }
        }

		#endregion
    }
}