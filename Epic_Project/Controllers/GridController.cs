using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic_Project.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPICProject.Controllers
{
    [Authorize]
    public class GridController : Controller
    {
        private readonly IRepository _repository;

        public GridController(IRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Editing_InLine()
        {
            PopulateEpicTypes();
            PopulateModules();
            PopulateMurabahas();
            PopulateFirstSellableModules();
            PopulateProjectLocations();
            PopulateTeams();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_repository.GetEpicBaseLineAll().ToDataSourceResult(request));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, EpicBaseLine epicBaseLine, string name)
        {
            if (epicBaseLine != null && ModelState.IsValid)
            {
                _repository.InsertEpicBaseLine(epicBaseLine);
            }

            return Json(new[] { epicBaseLine }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, EpicBaseLine epicBaseLine, string name)
        {
            if (epicBaseLine != null && ModelState.IsValid)
            {
                _repository.UpdateEpicBaseLine(epicBaseLine);
                //productService.Update(product);
            }

            return Json(new[] { epicBaseLine }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, EpicBaseLine epicBaseLine, string name)
        {
            if (epicBaseLine != null)
            {
                _repository.DeleteEpicBaseLine(epicBaseLine.EPICId);
                //productService.Destroy(epicBaseLine);
            }

            return Json(new[] { epicBaseLine }.ToDataSourceResult(request, ModelState));
        }


        private void PopulateEpicTypes()
        {
            List<EpicTypeViewModel> typeList = new List<EpicTypeViewModel>();
            List<Parameter> parameterList = (List<Parameter>)_repository.GetParameter("EpicType");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new EpicTypeViewModel();
                temp.TypeName = parameterList[i].ParameterName;
                temp.TypeValue = parameterList[i].ParameterValue;
                typeList.Add(temp);
            }
            ViewData["types"] = typeList;
            ViewData["defaultType"] = typeList.First();
        }

        private void PopulateModules()
        {
            IEnumerable<Module> moduleList = new List<Module>();
            moduleList = _repository.GetModuleAll();
            ViewData["modules"] = moduleList;
            ViewData["defaultModule"] = moduleList.First();
        }

        private void PopulateMurabahas()
        {
            List<MurabahaViewModel> murabahaList = new List<MurabahaViewModel>();
            List<Parameter> parameterList;
            parameterList = (List<Parameter>)_repository.GetParameter("IsMurabaha");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new MurabahaViewModel();
                temp.MurabahaName = parameterList[i].ParameterName;
                temp.MurabahaValue = parameterList[i].ParameterValue;
                murabahaList.Add(temp);
            }
            ViewData["murabahas"] = murabahaList;
            ViewData["defaultMurabaha"] = murabahaList.First();
        }
        private void PopulateFirstSellableModules()
        {
            List<IsFirstSellableModuleViewModel> firstSellableModuleList = new List<IsFirstSellableModuleViewModel>();
            List<Parameter> parameterList;
            parameterList = (List<Parameter>)_repository.GetParameter("IsFirstSellableModule");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new IsFirstSellableModuleViewModel();
                temp.FirstSellableModuleName = parameterList[i].ParameterName;
                temp.FirstSellableModuleValue = parameterList[i].ParameterValue;
                firstSellableModuleList.Add(temp);
            }
            ViewData["firstSellableModules"] = firstSellableModuleList;
            ViewData["defaultFirstSellableModule"] = firstSellableModuleList.First();
        }
        private void PopulateProjectLocations()
        {
            List<ProjectLocationViewModel> projectLocationList = new List<ProjectLocationViewModel>();
            List<Parameter> parameterList;
            parameterList = (List<Parameter>)_repository.GetParameter("ProjectLocation");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new ProjectLocationViewModel();
                temp.LocationName = parameterList[i].ParameterName;
                temp.LocationValue = parameterList[i].ParameterValue;
                projectLocationList.Add(temp);
            }
            ViewData["projectLocations"] = projectLocationList;
            ViewData["defaultLocation"] = projectLocationList.First();
        }
        private void PopulateTeams()
        {
            IEnumerable<Team> teamList = new List<Team>();
            teamList = _repository.GetTeamAll();
            ViewData["teams"] = teamList;
            ViewData["defaultTeam"] = new Team() { TeamId = 0, TeamName = "", ProjectManager = new EmployeeViewModel() { EmployeeId = 0, EmployeeName = ""}, TeamLeader = new EmployeeViewModel() { EmployeeId = 0, EmployeeName = "" } };
        }

    }
}