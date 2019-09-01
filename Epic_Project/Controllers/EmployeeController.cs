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

        public EmployeeController(IRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Editing_InLine()
        {
            PopulateEmployeeTypes();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_repository.GetEmployeeAll().ToDataSourceResult(request));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Employee employee)
        {
            if (employee != null && ModelState.IsValid)
            {
                _repository.InsertEmployee(employee);
            }

            return Json(new[] { employee }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Employee employee)
        {
            if (employee != null && ModelState.IsValid)
            {
                _repository.UpdateEmployee(employee);
            }

            return Json(new[] { employee }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Employee employee)
        {
            if (employee != null)
            {
                _repository.DeleteEmployee(employee.EmployeeId);
            }

            return Json(new[] { employee }.ToDataSourceResult(request, ModelState));
        }

        private void PopulateEmployeeTypes()
        {
            List<EmployeeTypeViewModel> typeList = new List<EmployeeTypeViewModel>();
            List<Parameter> parameterList = (List<Parameter>)_repository.GetParameter("EmployeeType");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new EmployeeTypeViewModel();
                temp.TypeName = parameterList[i].ParameterName;
                temp.TypeId = parameterList[i].ParameterValue;
                typeList.Add(temp);
            }
            ViewData["employeeTypes"] = typeList;
            ViewData["defaultEmployeeType"] = typeList.First();
        }
    }
}