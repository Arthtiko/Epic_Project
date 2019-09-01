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
    public class TeamController : Controller
    {
        private readonly IRepository _repository;

        public TeamController(IRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Editing_InLine()
        {
            PopulateTeamLeaders();
            PopulateProjectManagers();
            return View();
        }
        
        [Authorize(Roles = "Admin")]
        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_repository.GetTeamAll().ToDataSourceResult(request));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Team team)
        {
            if (team != null && ModelState.IsValid)
            {
                _repository.InsertTeam(team);
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Team team)
        {
            if (team != null && ModelState.IsValid)
            {
                _repository.UpdateTeam(team);
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Team team)
        {
            if (team != null)
            {
                _repository.DeleteTeam(team.TeamId);
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }
        

        private void PopulateTeamLeaders()
        {
            List<EmployeeViewModel> employeeList = new List<EmployeeViewModel>();
            // 2 -> Team Leader
            // 3 -> Project Manager
            List<Employee> employees = (List<Employee>)_repository.GetEmployeeByType(2);
            for (int i = 0; i < employees.Count(); i++)
            {
                EmployeeViewModel temp = new EmployeeViewModel();
                temp.EmployeeId = employees[i].EmployeeId;
                temp.EmployeeName = employees[i].EmployeeName;
                employeeList.Add(temp);
            }
            ViewData["teamLeaders"] = employeeList;
            ViewData["defaultTeamLeader"] = new EmployeeViewModel() { EmployeeId = 0, EmployeeName = "" };
        }

        private void PopulateProjectManagers()
        {
            List<EmployeeViewModel> employeeList = new List<EmployeeViewModel>();
            // 2 -> Team Leader
            // 3 -> Project Manager
            List<Employee> employees = (List<Employee>)_repository.GetEmployeeByType(3);
            for (int i = 0; i < employees.Count(); i++)
            {
                EmployeeViewModel temp = new EmployeeViewModel();
                temp.EmployeeId = employees[i].EmployeeId;
                temp.EmployeeName = employees[i].EmployeeName;
                employeeList.Add(temp);
            }
            ViewData["projectManagers"] = employeeList;
            ViewData["defaultProjectManager"] = new EmployeeViewModel() { EmployeeId = 0, EmployeeName = "" };
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}