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
    public class MeasurementGridController : Controller
    {
        private readonly IRepository _repository;
        private List<MeasurementDetailsViewModel> MeasurementDetailList = new List<MeasurementDetailsViewModel>();
        private List<Date> DateList = new List<Date>();
        private List<Team> TeamList = new List<Team>();
        private int StartMonth;
        private int StartYear;
        private int NextMonth;
        private int NextYear;
        public MeasurementGridController(IRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Editing_InLine(int epicId, int year, int month, string yearMonth, string location, string type, string teamName)
        {
            DateList = _repository.GetDates();
            if (DateList != null && DateList.Count > 0)
            {
                StartMonth = DateList[0].Month;
                StartYear = DateList[0].Year;
                int nm = DateList[0].Month;
                int ny = DateList[0].Year;
                if (nm >= 12)
                {
                    nm = 1;
                    ny++;
                }
                else
                {
                    nm++;
                }
                NextMonth = nm;
                NextYear = ny;
            }
            int m;
            int y;
            PopulateTeams();
            PopulateModules();
            PopulateTypes();
            PopulateDates();
            if (yearMonth == null)
            {
                m = month <= 0 || month > 12 ? StartMonth : month;
                y = year < 2000 || year > 9999 ? StartYear : year;
            }
            else
            {
                string yText = yearMonth.Split("-")[0];
                y = Convert.ToInt32(yText);
                string mText = yearMonth.Split("-")[1];
                m = Convert.ToInt32(mText);
            }
            teamName = teamName == null ? "" : teamName;
            location = location == null ? "" : location;
            type = type == null ? "" : type;
            var model = new MeasurementSearchModel() { EpicId = epicId, Year = y, Month = m, YearMonth = yearMonth, NextMonth = NextMonth, NextYear = NextYear, Location = location, Type = type, TeamName = teamName };
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ViewResult MeasurementSearch()
        {
            return View();
        }

        [Authorize]
        public IActionResult Editing_Inline_Details(int epicId, int year, int month, string yearMonth, string location, string type, string teamName)
        {
            DateList = _repository.GetDates();
            if (DateList != null && DateList.Count > 0)
            {
                StartMonth = DateList[0].Month;
                StartYear = DateList[0].Year;
                int nm = DateList[0].Month;
                int ny = DateList[0].Year;
                if (nm >= 12)
                {
                    nm = 1;
                    ny++;
                }
                else
                {
                    nm++;
                }
                NextMonth = nm;
                NextYear = ny;
            }
            int m;
            int y;
            if (yearMonth == null)
            {
                m = month <= 0 || month > 12 ? StartMonth : month;
                y = year < 2000 || year > 9999 ? StartYear : year;
            }
            else
            {
                string yText = yearMonth.Split("-")[0];
                y = Convert.ToInt32(yText);
                string mText = yearMonth.Split("-")[1];
                m = Convert.ToInt32(mText);
            }
            teamName = teamName == null ? "" : teamName;
            location = location == null ? "" : location;
            type = type == null ? "" : type;
            var model = new MeasurementSearchModel() { EpicId = epicId, Year = y, Month = m, YearMonth = yearMonth, NextMonth = NextMonth, NextYear = NextYear, Location = location, Type = type, TeamName = teamName };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult MeasurementSearch(int epicId, int year, int month)
        {
            return RedirectToAction("Editing_Inline_Details", new { epicId, year, month });
        }

        [Authorize]
        public ActionResult EditingInLineDetails_Read([DataSourceRequest] DataSourceRequest request, int epicId, int year, int month)
        {
            return Json(_repository.FillMeasurementDetails(epicId, year, month).ToDataSourceResult(request));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request, int epicId, int year, int month, string location, string type, string teamName)
        {
            var y = Json(_repository.SearchMeasurement(year, month, location, type, teamName).ToDataSourceResult(request));
            //var x = Json(_repository.GetMeasurementAll(epicId, year, month, null).ToDataSourceResult(request));
            return y;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Measurement measurement)
        {
            Measurement newMeasurement = new Measurement();
            if (measurement != null && ModelState.IsValid)
            {
                if (measurement.Module.ModuleName == null)
                {
                    measurement.Module.ModuleName = "";
                }
                if (measurement.Team.TeamName == null)
                {
                    measurement.Team.TeamName = "";
                    measurement.Team.TeamLeader = new EmployeeViewModel();
                    measurement.Team.ProjectManager = new EmployeeViewModel();
                }
                Measurement temp = _repository.InsertMeasurement(measurement);
                if (temp != null)
                {
                    newMeasurement = _repository.GetMeasurementAll(measurement.EpicId, measurement.Year, measurement.Month, measurement.Type.TypeName).First();
                }
            }

            return Json(new[] { newMeasurement }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Measurement measurement)
        {
            Measurement newMeasurement = new Measurement();
            if (measurement != null && ModelState.IsValid)
            {
                if (DateTime.Today.Month - measurement.Month >= 2)
                {
                    ViewBag.EditErrorMessage = "You can only edit the measurements of previous month!";
                    return null;
                }
                newMeasurement = _repository.UpdateMeasurement(measurement);
                if (newMeasurement.Team.TeamId == 0)
                {
                    newMeasurement.Team.TeamName = "";
                    newMeasurement.Team.TeamLeader = new EmployeeViewModel();
                    newMeasurement.Team.ProjectManager = new EmployeeViewModel();
                }
            }

            return Json(new[] { newMeasurement }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Measurement measurement)
        {
            if (measurement != null)
            {
                _repository.DeleteMeasurement(measurement.EpicId, measurement.Year, measurement.Month, _repository.GetParameterValue("Type" ,measurement.Type.TypeName));
            }

            return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GenerateNextMonth(int year, int month, string location)
        {
            //location null yapıldı
            _repository.GenerateMeasurementForNextMonth(year, month, null);
            return RedirectToAction("Editing_Inline", "MeasurementGrid");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteLastMonth(int year, int month, string location)
        {
            _repository.DeleteLastMonth(year, month, location);
            return RedirectToAction("Editing_Inline", "MeasurementGrid");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        private void PopulateTypes()
        {
            List<MeasurementTypeViewModel> typeList = new List<MeasurementTypeViewModel>();
            List<Parameter> parameterList = (List<Parameter>)_repository.GetParameter("Type");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new MeasurementTypeViewModel();
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
            ViewData["defaultModule"] = new Module() { ModuleId = 0, ModuleName = ""};
        }
        private void PopulateTeams()
        {
            List<Team> teamList = new List<Team>();
            List<Team> temp = new List<Team>();
            temp = (List<Team>)_repository.GetTeamAll();
            teamList.Add(new Team { TeamId = 0, TeamName = "", TeamLeader = new EmployeeViewModel(), ProjectManager = new EmployeeViewModel() });
            for (int i = 0; i < temp.Count(); i++)
            {
                teamList.Add(temp[i]);
            }
            ViewData["teams"] = teamList;
            ViewData["defaultTeam"] = new Team() { TeamId = 0, TeamName = ""};
        }

        private void PopulateDates()
        {
            DateList = _repository.GetDates();
            int count = DateList.Count();
            string[] dates = new string[count];
            string[] options = new string[count];

            for (int i = 0; i < count; i++)
            {
                dates[i] = DateList[i].Year.ToString() + "-" + DateList[i].Month.ToString();
                options[i] = dates[i].ToString() + "|" + dates[i].ToString();
            }
            if (DateList[0] != null)
            {
                int month = DateList[0].Month;
                int year = DateList[0].Year;
                if (month >= 12)
                {
                    ViewData["nextMonth"] = 1;
                    ViewData["nextYear"] = year++;
                }
                else
                {
                    ViewData["nextMonth"] = month;
                    ViewData["nextYear"] = year;
                }
            }
            ViewData["dates"] = options;
        }

        public JsonResult selectDates()
        {
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
        public JsonResult SelectTeams()
        {
            TeamList = (List<Team>)_repository.GetTeamAll();
            List<string> selectList = new List<string>();
            selectList.Add("");
            for (int i = 0; i < TeamList.Count(); i++)
            {
                selectList.Add(TeamList[i].TeamName);
            }
            return Json(selectList);
        }

    }
}