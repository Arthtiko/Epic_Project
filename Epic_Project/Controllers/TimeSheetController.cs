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
    [Authorize(Roles = "Admin, Project Manager, Program Manager")]
    public class TimeSheetController : Controller
    {
        private readonly IRepository _repository;

        #region Constructor

        public TimeSheetController(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Views
        public IActionResult TimeSheetTable()
        {
            return View();
        }
        public IActionResult TimeSheetImport()
        {
            return View();
        }

        #endregion

        #region Reads

        public ActionResult TimeSheetTable_Read([DataSourceRequest] DataSourceRequest request, string name, string project, string task)
        {
            return Json(_repository.GetTimeSheetAll(name, project, task).ToDataSourceResult(request));
        }

        #endregion

        #region Creates
        
        [HttpPost]
        public ActionResult TimeSheetTable_Create([DataSourceRequest] DataSourceRequest request, TimeSheet timeSheet)
        {
            if (timeSheet != null && ModelState.IsValid)
            {
                _repository.InsertTimeSheetAll(timeSheet);
            }
            return Json(new[] { timeSheet }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Updates
        
        [HttpPost]
        public ActionResult TimeSheetTable_Update([DataSourceRequest] DataSourceRequest request, TimeSheet timeSheet)
        {
            Measurement newMeasurement = new Measurement();
            if (timeSheet != null && ModelState.IsValid)
            {
                _repository.UpdateTimeSheet(timeSheet);
            }

            return Json(new[] { timeSheet }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Deletes

        [HttpPost]
        public ActionResult TimeSheetTable_Destroy([DataSourceRequest] DataSourceRequest request, TimeSheet timeSheet)
        {
            if (timeSheet != null)
            {

                _repository.DeleteTimeSheetAll(timeSheet);
            }

            return Json(new[] { timeSheet }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Import 

        public string ImportTimeSheet(string name, string project, string task, float hour, string date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            dateTime = dateTime.AddDays(1);
            if (dateTime == null || dateTime == Convert.ToDateTime("01-01-0001"))
            {
                return "Invalid date";
            }
            TimeSheet timeSheet = new TimeSheet();
            timeSheet = new TimeSheet()
            {
                Name = name,
                Project = project,
                Task = task,
                Hour = hour,
                Date = dateTime
            };

            if (timeSheet != null)
            {
                _repository.InsertTimeSheetAll(timeSheet);
                return "Success";
            }
            else
            {
                return "Couldn't construct Time Sheet";
            }
        }

        #endregion

        #region Side Operations

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Index_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            return File(fileContents, contentType, fileName);
        }

        #endregion
    }
}