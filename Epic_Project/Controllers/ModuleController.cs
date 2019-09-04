using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic_Project.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epic_Project.Controllers
{
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly IRepository _repository;

        public ModuleController(IRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Editing_InLine()
        {
            var model = new MaxIdViewModel(){ MaxId = _repository.GetMaxModuleId() + 1 };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_repository.GetModuleAll().ToDataSourceResult(request));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Module module)
        {
            if (module != null && ModelState.IsValid)
            {
                Module m = _repository.InsertModule(module);
                
            }
            return Json(new[] { module }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Module module)
        {
            if (module != null && ModelState.IsValid)
            {
                _repository.UpdateModule(module);
            }

            return Json(new[] { module }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Module module)
        {
            if (module != null)
            {
                _repository.DeleteModule(module.ModuleId);
            }

            return Json(new[] { module }.ToDataSourceResult(request, ModelState));
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}