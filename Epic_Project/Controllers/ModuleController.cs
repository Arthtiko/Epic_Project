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

        [Authorize]
        public ActionResult Editing_InLine()
        {
            return View();
        }

        [Authorize]
        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = _repository.GetDates()[0];
            return Json(_repository.GetModuleProgress(year, month).ToDataSourceResult(request));
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Module module)
        {
            if (module != null && ModelState.IsValid)
            {
                Module m = _repository.InsertModule(module);
            }

            return Json(new[] { module }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Module module)
        {
            if (module != null && ModelState.IsValid)
            {
                _repository.UpdateModule(module);
            }

            return Json(new[] { module }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Module module)
        {
            if (module != null)
            {
                _repository.DeleteModule(module.ModuleId);
            }

            return Json(new[] { module }.ToDataSourceResult(request, ModelState));
        }

        public string IsVarianceShowed(int year, int month)
        {
            List<DateControl> dateControlList = (List<DateControl>)_repository.GetDateControl(year, month, null);
            bool isVarianceShowed = true;
            int idx = 0;
            for (int i = 0; i < dateControlList.Count(); i++)
            {
                if (dateControlList[i].Variance.VarianceName == "TRUE")
                {
                    idx++;
                }
            }
            if (idx != dateControlList.Count())
            {
                isVarianceShowed = false;
            }
            if (isVarianceShowed)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public JsonResult selectDates()
        {
            List<Date> DateList = new List<Date>();
            int StartMonth;
            int StartYear;
            int NextMonth;
            int NextYear;
            DateList = _repository.GetDates();
            List<string> selectList = new List<string>();
            for (int i = 0; i < DateList.Count(); i++)
            {
                selectList.Add(DateList[i].Year.ToString() + "-" + DateList[i].Month.ToString());
            }
            if (selectList != null || selectList.Count() > 0)
            {
                int m = DateList[0].Month;
                int y = DateList[0].Year;
                if (m >= 12)
                {
                    m = 1;
                    y++;
                }
                else
                {
                    m++;
                }
                NextMonth = m;
                NextYear = y;
            }
            else
            {
                int m = DateTime.Today.Month;
                int y = DateTime.Today.Year;
                if (m >= 12)
                {
                    m = 1;
                    y++;
                }
                else
                {
                    m++;
                }
                NextMonth = m;
                NextYear = y;
            }
            return Json(selectList);
        }
    }
}