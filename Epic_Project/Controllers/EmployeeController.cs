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
    public class EmployeeController : Controller
    {
        private readonly IRepository _repository;

        #region Constructor

        public EmployeeController(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Views

        public ActionResult Editing_InLine()
        {
            PopulateEmployeeTypes();
            PopulateEmployeeLocations();
            return View();
        }

        #endregion

        #region Reads

        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_repository.GetEmployeeAll().ToDataSourceResult(request));
        }

        #endregion

        #region Creates

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Employee employee)
        {
            if (employee != null && ModelState.IsValid)
            {
                _repository.InsertEmployee(employee);
            }
            return Json(new[] { employee }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Updates

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Employee employee)
        {
            if (employee != null && ModelState.IsValid)
            {
                _repository.UpdateEmployee(employee);
            }
            return Json(new[] { employee }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Deletes

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Employee employee)
        {
            if (employee != null)
            {
                _repository.DeleteEmployee(employee.EmployeeId);
            }

            return Json(new[] { employee }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Operations

        private void PopulateEmployeeTypes()
        {
            List<EmployeeTypeViewModel> typeList = new List<EmployeeTypeViewModel>();
            List<Parameter> parameterList = (List<Parameter>)_repository.GetParameter("EmployeeType");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new EmployeeTypeViewModel
                {
                    TypeName = parameterList[i].ParameterName,
                    TypeId = parameterList[i].ParameterValue
                };
                typeList.Add(temp);
            }
            ViewData["employeeTypes"] = typeList;
            ViewData["defaultEmployeeType"] = typeList.First();
        }

        private void PopulateEmployeeLocations()
        {
            List<ProjectLocationViewModel> employeeLocationList = new List<ProjectLocationViewModel>();
            List<Parameter> parameterList;
            parameterList = (List<Parameter>)_repository.GetParameter("ProjectLocation");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new ProjectLocationViewModel
                {
                    LocationName = parameterList[i].ParameterName,
                    LocationValue = parameterList[i].ParameterValue
                };
                employeeLocationList.Add(temp);
            }
            ViewData["employeeLocations"] = employeeLocationList;
            ViewData["defaultEmployeeLocation"] = employeeLocationList.First();
        }

        #endregion
    }
}