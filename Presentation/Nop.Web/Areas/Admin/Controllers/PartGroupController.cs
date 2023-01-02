using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PartGroupController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryModelFactory _categoryModelFactory;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public PartGroupController(ICategoryModelFactory categoryModelFactory,
            IPermissionService permissionService)
        {
            _categoryModelFactory = categoryModelFactory;
            _permissionService = permissionService;
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = _categoryModelFactory.PreparePartGroupSearchModelAsync(new PartGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(PartGroupSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _categoryModelFactory.PreparePartGroupListModelAsync(searchModel);

            return Json(model);
        }

        #endregion
        
    }
}