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

        #region Constructor

        public TeamController(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Views

        public ActionResult Editing_InLine()
        {
            PopulateTeamLeaders();
            PopulateProjectManagers();
            return View();
        }

        #endregion

        #region Reads

        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_repository.GetTeamAll(0, null, 0, 0).ToDataSourceResult(request));
        }

        #endregion

        #region Creates

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Team team)
        {
            if (team != null && ModelState.IsValid)
            {
                _repository.InsertTeam(team);
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Updates

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Team team)
        {
            if (team != null && ModelState.IsValid)
            {
                _repository.UpdateTeam(team);
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Deletes

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Team team)
        {
            if (team != null)
            {
                _repository.DeleteTeam(team.TeamId);
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Operations

        private void PopulateTeamLeaders()
        {
            List<TeamLeaderViewModel> employeeList = new List<TeamLeaderViewModel>();
            int typeId = _repository.GetParameterValue("EmployeeType", "Team Leader");
            List<Employee> employees = (List<Employee>)_repository.GetEmployeeByType(typeId);
            for (int i = 0; i < employees.Count(); i++)
            {
                TeamLeaderViewModel temp = new TeamLeaderViewModel();
                temp.TeamLeaderId = employees[i].EmployeeId;
                temp.TeamLeaderName = employees[i].EmployeeName;
                employeeList.Add(temp);
            }
            ViewData["teamLeaders"] = employeeList;
            ViewData["defaultTeamLeader"] = new TeamLeaderViewModel() { TeamLeaderId = 0, TeamLeaderName = "" };
        }

        private void PopulateProjectManagers()
        {
            List<ProjectManagerViewModel> employeeList = new List<ProjectManagerViewModel>();
            int typeId = _repository.GetParameterValue("EmployeeType", "Project Manager");
            List<Employee> employees = (List<Employee>)_repository.GetEmployeeByType(typeId);
            for (int i = 0; i < employees.Count(); i++)
            {
                ProjectManagerViewModel temp = new ProjectManagerViewModel();
                temp.ProjectManagerId = employees[i].EmployeeId;
                temp.ProjectManagerName = employees[i].EmployeeName;
                employeeList.Add(temp);
            }
            ViewData["projectManagers"] = employeeList;
            ViewData["defaultProjectManager"] = new ProjectManagerViewModel() { ProjectManagerId = 0, ProjectManagerName = "" };
        }

        [HttpPost]
        public ActionResult GetTurkeyProgress()
        {
            Date date = _repository.GetDates()[0];
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, "Turkey", null);
            model.Completed = (float)Math.Round(model.Completed, 2);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, 2);
            model.Variance = (float)Math.Round((float)model.Variance, 2);
            model.Total = _repository.GetEpicWeight("Turkey", null);
            model.Total = (float)Math.Round(model.Total * 100, 2);
            IEnumerable<ProgressModel> progressList = new ProgressModel[]
            {
                model
            };
            return Json(progressList);
        }

        #endregion
    }
}