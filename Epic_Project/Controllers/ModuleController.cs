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

        #region Constructor

        public ModuleController(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Views

        public ActionResult Editing_InLine()
        {
            return View();
        }

        #endregion

        #region Reads

        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request, int year, int month, int fsm, string location)
        {
            return Json(_repository.GetModuleProgress(year, month, fsm, location == "All" ? null : location).ToDataSourceResult(request));
        }

        #endregion

        #region Creates

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

        #endregion

        #region Updates

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

        #endregion

        #region Deletes

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

        #endregion

        #region Operations

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public string GetEpicCount(string location)
        {
            if (location == "All")
            {
                location = null;
            }
            List<Module> list = _repository.GetModuleAggregates(0, 0, 0, location);
            int totalCount = 0;
            for (int i = 0; i < list.Count(); i++)
            {
                totalCount += list[i].EpicCount;
            }
            return totalCount.ToString();
        }

        public string IsVarianceShowed(int year, int month)
        {
            List<DateControl> dateControlList = (List<DateControl>)_repository.GetDateControl(year, month, null);
            bool isVarianceShowed = true;
            bool isEffortShowed = true;
            int idxVar = 0; int idxEffort = 0;
            for (int i = 0; i < dateControlList.Count(); i++)
            {
                if (dateControlList[i].Variance.VarianceName == "TRUE")
                {
                    idxVar++;
                    idxEffort++;
                }
            }
            if (idxVar != dateControlList.Count())
            {
                isVarianceShowed = false;
            }
            if (idxEffort != dateControlList.Count())
            {
                isEffortShowed = false;
            }
            if (isVarianceShowed && isEffortShowed)
            {
                return "11";
            }
            else if (isVarianceShowed && !isEffortShowed)
            {
                return "10";
            }
            else if (!isVarianceShowed && isEffortShowed)
            {
                return "01";
            }
            else
            {
                return "00";
            }
        }

        public JsonResult selectDates()
        {
            List<Date> DateList = new List<Date>();
            DateList = _repository.GetDates();
            List<string> selectList = new List<string>();
            for (int i = 0; i < DateList.Count(); i++)
            {
                selectList.Add(DateList[i].Year.ToString() + "-" + DateList[i].Month.ToString());
            }
            return Json(selectList);
        }

        public JsonResult selectFSM()
        {
            List<string> list = new List<string>();
            list.Add("Overall");
            list.Add("First Sellable Module");
            //list.Add("Not First Sellable Module");

            return Json(list);
        }

        public JsonResult selectLocations()
        {
            List<string> locations = new List<string>()
            {
                "All", "Turkey", "Egypt"
            };
            return Json(locations);
        }

        #endregion
    }
}