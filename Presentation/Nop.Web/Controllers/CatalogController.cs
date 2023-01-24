using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogController(CatalogSettings catalogSettings,
            IAclService aclService,
            ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _catalogModelFactory = catalogModelFactory;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _permissionService = permissionService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _productTagService = productTagService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utility

        public class ApiModel
        {
            public int id { get; set; }
            public string value { get; set; }
        }

        private async Task<List<SelectListItem>> PrepareMakeDropdownAsync(int yearId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Select Make", Value = "0" });

            if (yearId > 0)
            {
                var makes = await _productService.GetAllMakesAsync(yearId: yearId);
                foreach (var make in makes)
                    list.Add(new SelectListItem { Text = make.Name, Value = make.Id.ToString() });
            }
            return list;
        }

        private async Task<List<SelectListItem>> PrepareModelDropdownAsync(int yearId, int makeId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Select Model", Value = "0" });

            if (yearId > 0 && makeId > 0)
            {
                var models = await _productService.GetAllModelsAsync(yearId: yearId, makeId: makeId);
                foreach (var model in models)
                    list.Add(new SelectListItem { Text = model.Name, Value = model.Id.ToString() });
            }
            return list;
        }

        private async Task<List<SelectListItem>> PrepareEngineDropdownAsync(int yearId, int makeId, int modelId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Select Engine", Value = "0" });

            if (yearId > 0 && makeId > 0 && modelId > 0)
            {
                var engines = await _productService.GetAllEnginesAsync(yearId: yearId, makeId: makeId, modelId: modelId);
                foreach (var engine in engines)
                    list.Add(new SelectListItem { Text = engine.Name, Value = engine.Id.ToString() });
            }
            return list;
        }

        private async Task<List<SelectListItem>> PreparePartGroupDropdownAsync()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Select Part Group", Value = "0" });

            var partGroups = await _categoryService.GetAllPartGroupsAsync();
            foreach (var partGroup in partGroups)
                list.Add(new SelectListItem { Text = partGroup.Name, Value = partGroup.Id.ToString() });

            return list;
        }
        private async Task<List<SelectListItem>> PreparePartTypesDropdownAsync(int partGroupId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Select Part Type", Value = "0" });

            var partTypes = await _categoryService.GetAllPartTypesAsync(partGroupId: partGroupId);
            foreach (var partType in partTypes)
                list.Add(new SelectListItem { Text = partType.Name, Value = partType.Id.ToString() });

            return list;
        }

        #endregion

        #region Categories

        public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ViewCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            //model
            var model = await _catalogModelFactory.PrepareCategoryModelAsync(category, command);

            //template
            var templateViewPath = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return NotFound();

            var model = await _catalogModelFactory.PrepareCategoryProductsModelAsync(category, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogRoot()
        {
            var model = await _catalogModelFactory.PrepareRootCategoriesAsync();

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogSubCategories(int id)
        {
            var model = await _catalogModelFactory.PrepareSubCategoriesAsync(id);

            return Json(model);
        }

        #endregion

        #region Part group

        public virtual async Task<IActionResult> PartGroup(int partGroupId, CatalogProductsCommand command)
        {
            var partGroup = await _categoryService.GetPartGroupByIdAsync(partGroupId);
            if (partGroup is null)
                return InvokeHttp404();

            //model
            var model = new PartGroupModel();
            model.Name = partGroup.Name;

            var partTypes = await _categoryService.GetAllPartTypesAsync(partGroupId:partGroupId);
            foreach (var partType in partTypes)
                model.PartTypes.Add(new PartGroupModel.PartTypeModel { Id = partType.Id, Name = partType.Name });

            return View(model);
        }

        #endregion

        #region Manufacturers

        public virtual async Task<IActionResult> Manufacturer(int manufacturerId, CatalogProductsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.Admin }));

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ViewManufacturer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

            //model
            var model = await _catalogModelFactory.PrepareManufacturerModelAsync(manufacturer, command);

            //template
            var templateViewPath = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(manufacturer.ManufacturerTemplateId);

            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetManufacturerProducts(int manufacturerId, CatalogProductsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return NotFound();

            var model = await _catalogModelFactory.PrepareManufacturerProductsModelAsync(manufacturer, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> ManufacturerAll()
        {
            var model = await _catalogModelFactory.PrepareManufacturerAllModelsAsync();

            return View(model);
        }

        #endregion

        #region Vendors

        public virtual async Task<IActionResult> Vendor(int vendorId, CatalogProductsCommand command)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.Admin }));

            //model
            var model = await _catalogModelFactory.PrepareVendorModelAsync(vendor, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetVendorProducts(int vendorId, CatalogProductsCommand command)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return NotFound();

            var model = await _catalogModelFactory.PrepareVendorProductsModelAsync(vendor, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("Homepage");

            var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();
            return View(model);
        }

        #endregion

        #region Product tags

        public virtual async Task<IActionResult> ProductsByTag(int productTagId, CatalogProductsCommand command)
        {
            var productTag = await _productTagService.GetProductTagByIdAsync(productTagId);
            if (productTag == null)
                return InvokeHttp404();

            var model = await _catalogModelFactory.PrepareProductsByTagModelAsync(productTag, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetTagProducts(int tagId, CatalogProductsCommand command)
        {
            var productTag = await _productTagService.GetProductTagByIdAsync(tagId);
            if (productTag == null)
                return NotFound();

            var model = await _catalogModelFactory.PrepareTagProductsModelAsync(productTag, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> ProductTagsAll()
        {
            var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();

            return View(model);
        }

        #endregion

        #region Searching

        public virtual async Task<IActionResult> Search(SearchModel model, CatalogProductsCommand command)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                store.Id);

            if (model == null)
                model = new SearchModel();

            model = await _catalogModelFactory.PrepareSearchModelAsync(model, command);

            return View(model);
        }
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Content("");

            term = term.Trim();

            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;
            var store = await _storeContext.GetCurrentStoreAsync();
            var products = await _productService.SearchProductsAsync(0,
                storeId: store.Id,
                keywords: term,
                languageId: (await _workContext.GetWorkingLanguageAsync()).Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

            var models = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize)).ToList();
            var result = (from p in models
                          select new
                          {
                              label = p.Name,
                              producturl = Url.RouteUrl("Product", new { SeName = p.SeName }),
                              productpictureurl = p.DefaultPictureModel.ImageUrl,
                              showlinktoresultsearch = showLinkToResultSearch
                          })
                .ToList();
            return Json(result);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> SearchProducts(SearchModel searchModel, CatalogProductsCommand command)
        {
            if (searchModel == null)
                searchModel = new SearchModel();

            var model = await _catalogModelFactory.PrepareSearchProductsModelAsync(searchModel, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        #endregion

        #region Utilities

        private async Task<bool> CheckCategoryAvailabilityAsync(Category category)
        {
            var isAvailable = true;

            if (category == null || category.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(category) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        private async Task<bool> CheckManufacturerAvailabilityAsync(Manufacturer manufacturer)
        {
            var isAvailable = true;

            if (manufacturer == null || manufacturer.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(manufacturer) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        private Task<bool> CheckVendorAvailabilityAsync(Vendor vendor)
        {
            var isAvailable = true;

            if (vendor == null || vendor.Deleted || !vendor.Active)
                isAvailable = false;

            return Task.FromResult(isAvailable);
        }

        public async Task<IActionResult> GetMakes(int yearId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var token = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.XAuthToken);

            var list = await PrepareMakeDropdownAsync(yearId);
            return Json(new
            {
                success = true,
                list
            });
        }

        public async Task<IActionResult> GetModels(int yearId, int makeId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var token = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.XAuthToken);

            var list = await PrepareModelDropdownAsync(yearId, makeId);
            return Json(new
            {
                success = true,
                list
            });
        }

        public async Task<IActionResult> GetEngines(int yearId, int makeId, int modelId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var token = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.XAuthToken);

            var list = await PrepareEngineDropdownAsync(yearId, makeId, modelId);
            return Json(new
            {
                success = true,
                list
            });
        }

        public async Task<IActionResult> GetPartTypes(int partGroupId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var token = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.XAuthToken);

            var list = await PreparePartTypesDropdownAsync(partGroupId);
            return Json(new
            {
                success = true,
                list
            });
        }

        public class PartGroups
        {
            public string id { get; set; }
            public string name { get; set; }
            public string engineCode { get; set; }
            public List<PartType> partTypes { get; set; }
        }

        public class PartType
        {
            public string id { get; set; }
            public string name { get; set; }
            public string groupId { get; set; }
        }

        public class Parts
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<PartGroups> partGroups { get; set; }
        }

        #endregion
    }
}