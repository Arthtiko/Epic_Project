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
    public class LogGridController : Controller
    {
        private readonly IRepository _repository;

        public LogGridController(IRepository repository)
        {
            _repository = repository;
        }
        

        public IActionResult Measurement()
        {
            return View();
        }
        public ActionResult Measurement_Read([DataSourceRequest] DataSourceRequest request, string epicId, int year, int month, string type, string user)
        {
            int id;
            if (epicId == "All")
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(epicId);
            }
            if (type == "All")
            {
                type = null;
            }
            if (user == "All")
            {
                user = null;
            }
            var x = _repository.GetLogMeasurement(id, year, month, type, user);
            return Json(x.ToDataSourceResult(request));
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager, Team Leader")]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
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

        public JsonResult selectIDs()
        {
            List<EpicBaseLine> epics = new List<EpicBaseLine>();
            epics = (List<EpicBaseLine>)_repository.GetEpicBaseLineAll(0);
            List<string> selectList = new List<string>();
            selectList.Add("All");
            for (int i = 0; i < epics.Count(); i++)
            {
                selectList.Add(epics[i].EPICId.ToString());
            }
            return Json(selectList);
        }

        public JsonResult selectTypes()
        {
            List<string> locations = new List<string>()
            {
                "All", "Actual", "Target"
            };
            return Json(locations);
        }

        public JsonResult selectUsers()
        {
            List<Employee> employees = new List<Employee>();
            employees = (List<Employee>)_repository.GetEmployeeAll();
            List<string> selectList = new List<string>();
            selectList.Add("All");
            for (int i = 0; i < employees.Count(); i++)
            {
                selectList.Add(employees[i].EmployeeName);
            }
            return Json(selectList);
        }
    }
}