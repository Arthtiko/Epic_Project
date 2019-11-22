using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Epic_Project.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPICProject.Controllers
{
    [Authorize]
    public class GridController : Controller
    {
        private IHttpContextAccessor _accessor;
        private readonly IRepository _repository;
        private string UserId;

        public GridController(IRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
            _repository = repository;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        [Authorize]
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

        [Authorize]
        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request)
        {
            var x = _repository.GetEpicBaseLineAll(0).ToDataSourceResult(request);
            return Json(x);
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, EpicBaseLine epicBaseLine, string name)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (epicBaseLine != null && ModelState.IsValid)
            {
                int id = _repository.GetEmployeeId(UserId);
                if (id == 1001)
                {
                    _repository.InsertEpicBaseLine(epicBaseLine, "Test Admin Turkey", ipAddress);
                }
                else if (id == 2001) {
                    _repository.InsertEpicBaseLine(epicBaseLine, "Test Admin Egypt", ipAddress);
                }
                else
                {
                    _repository.InsertEpicBaseLine(epicBaseLine, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
                }
            }
            return Json(new[] { epicBaseLine }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, EpicBaseLine epicBaseLine, string name)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (epicBaseLine != null && ModelState.IsValid)
            {
                int id = _repository.GetEmployeeId(UserId);
                if (id == 1001)
                {
                    _repository.UpdateEpicBaseLine(epicBaseLine, "Test Admin", ipAddress);
                }
                else if (id == 2001)
                {
                    _repository.InsertEpicBaseLine(epicBaseLine, "Test Admin Egypt", ipAddress);
                }
                else
                {
                    _repository.UpdateEpicBaseLine(epicBaseLine, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
                }
            }
            return Json(new[] { epicBaseLine }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, EpicBaseLine epicBaseLine, string name)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (epicBaseLine != null)
            {
                int id = _repository.GetEmployeeId(UserId);
                if (id == 1001)
                {
                    _repository.DeleteEpicBaseLine(epicBaseLine.EPICId, "Test Admin", ipAddress);
                }
                else if (id == 2001)
                {
                    _repository.InsertEpicBaseLine(epicBaseLine, "Test Admin Egypt", ipAddress);
                }
                else
                {
                    _repository.DeleteEpicBaseLine(epicBaseLine.EPICId, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
                }
            }
            return Json(new[] { epicBaseLine }.ToDataSourceResult(request, ModelState));
        }

        [Authorize]
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

        [Authorize]
        private void PopulateModules()
        {
            IEnumerable<Module> moduleList = new List<Module>();
            moduleList = _repository.GetModuleAll();
            ViewData["modules"] = moduleList;
            ViewData["defaultModule"] = moduleList.First();
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        private void PopulateTeams()
        {
            IEnumerable<Team> teamList = new List<Team>();
            teamList = _repository.GetTeamAll(0, null, 0, 0);
            ViewData["teams"] = teamList;
            ViewData["defaultTeam"] = new Team() { TeamId = 0, TeamName = "", ProjectManager = new ProjectManagerViewModel() { ProjectManagerId = 0, ProjectManagerName = ""}, TeamLeader = new TeamLeaderViewModel() { TeamLeaderId = 0, TeamLeaderName = "" } };
        }

        [Authorize]
        public JsonResult FilterMenu_Locations()
        {
            List<string> locations = new List<string>()
            {
                "Turkey", "Egypt"
            };
            return Json(locations);
        }

        [Authorize]
        public JsonResult FilterMenu_IsFirstSellableModule()
        {
            List<string> locations = new List<string>()
            {
                "TRUE", "FALSE"
            };
            return Json(locations);
        }

        [Authorize]
        public JsonResult FilterMenu_Type()
        {
            List<string> locations = new List<string>()
            {
                "Architecture", "Business", "Shared"
            };
            return Json(locations);
        }

    }
}