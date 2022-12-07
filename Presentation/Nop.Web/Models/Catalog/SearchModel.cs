using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableYears = new List<SelectListItem>();
            AvailableMakes = new List<SelectListItem>();
            AvailableModels = new List<SelectListItem>();
            AvailableEngine = new List<SelectListItem>();
            CatalogProductsModel = new CatalogProductsModel();
        }

        /// <summary>
        /// Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        public string q { get; set; }

        /// <summary>
        /// Category ID
        /// </summary>
        [NopResourceDisplayName("Search.Category")]
        public int cid { get; set; }

        [NopResourceDisplayName("Search.IncludeSubCategories")]
        public bool isc { get; set; }

        /// <summary>
        /// Manufacturer ID
        /// </summary>
        [NopResourceDisplayName("Search.Manufacturer")]
        public int mid { get; set; }

        /// <summary>
        /// Vendor ID
        /// </summary>
        [NopResourceDisplayName("Search.Vendor")]
        public int vid { get; set; }

        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [NopResourceDisplayName("Search.SearchInDescriptions")]
        public bool sid { get; set; }

        /// <summary>
        /// A value indicating whether "advanced search" is enabled
        /// </summary>
        [NopResourceDisplayName("Search.AdvancedSearch")]
        public bool advs { get; set; }

        /// <summary>
        /// A value indicating whether "allow search by vendor" is enabled
        /// </summary>
        public bool asv { get; set; }

        [NopResourceDisplayName("Search.Year")]
        public int yid { get; set; }

        [NopResourceDisplayName("Search.Make")]
        public int maid { get; set; }

        [NopResourceDisplayName("Search.Model")]
        public int moid { get; set; }
        
        [NopResourceDisplayName("Search.Engine")]
        public int eid { get; set; }
        
        [NopResourceDisplayName("Search.VIN")]
        public int vin { get; set; }


        public CatalogProductsModel CatalogProductsModel { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public IList<SelectListItem> AvailableYears { get; set; }
        public IList<SelectListItem> AvailableMakes { get; set; }
        public IList<SelectListItem> AvailableModels { get; set; }
        public IList<SelectListItem> AvailableEngine { get; set; }

        #region Nested classes

        public record CategoryModel : BaseNopEntityModel
        {
            public string Breadcrumb { get; set; }
        }

        #endregion
    }
}