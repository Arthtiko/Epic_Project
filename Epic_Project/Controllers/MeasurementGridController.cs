﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Epic_Project.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPICProject.Controllers
{
    [Authorize]
    public class MeasurementGridController : Controller
    {
        private IHttpContextAccessor _accessor;
        private readonly IRepository _repository;
        private string UserId;
        private List<MeasurementDetailsViewModel> MeasurementDetailList = new List<MeasurementDetailsViewModel>();
        private List<Date> DateList = new List<Date>();
        private List<Team> TeamList = new List<Team>();
        private int StartMonth;
        private int StartYear;
        private int NextMonth;
        private int NextYear;

        #region Constructor

        public MeasurementGridController(IRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
            _repository = repository;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        #endregion

        #region Views

        public ActionResult Editing_InLine(int epicId, int year, int month, string yearMonth, string type, string teamName)
        {
            Employee emp;
            string location;
            int employeeId = _repository.GetEmployeeId(UserId);
            if (employeeId == 1001)
            {
                emp = new Employee() { EmployeeId = 1001, EmployeeName = "Test Admin Turkey", EmployeeType = new EmployeeTypeViewModel() { TypeName = "Project Manager" }, EmployeeLocation = new ProjectLocationViewModel() { LocationName = "Turkey" } };
                location = emp.EmployeeLocation.LocationName;
            }
            else if (employeeId == 2001)
            {
                emp = new Employee() { EmployeeId = 2001, EmployeeName = "Test Admin Egypt", EmployeeType = new EmployeeTypeViewModel() { TypeName = "Project Manager" }, EmployeeLocation = new ProjectLocationViewModel() { LocationName = "Egypt" } };
                location = emp.EmployeeLocation.LocationName;
            }
            else if (employeeId == 1003)
            {
                emp = new Employee() { EmployeeId = 1003, EmployeeName = "Test Tester Turkey", EmployeeType = new EmployeeTypeViewModel() { TypeName = "Project Manager" }, EmployeeLocation = new ProjectLocationViewModel() { LocationName = "Turkey" } };
                location = emp.EmployeeLocation.LocationName;
            }
            else if (employeeId == 2003)
            {
                emp = new Employee() { EmployeeId = 2003, EmployeeName = "Test Tester Egypt", EmployeeType = new EmployeeTypeViewModel() { TypeName = "Project Manager" }, EmployeeLocation = new ProjectLocationViewModel() { LocationName = "Egypt" } };
                location = emp.EmployeeLocation.LocationName;
            }
            else
            {
                emp = _repository.GetEmployeeById(employeeId);
                location = emp.EmployeeLocation.LocationName;
            }

            DateList = _repository.GetDates();
            int maxYear = DateList[0].Year;
            int maxMonth = DateList[0].Month;
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
            PopulateTypes();
            PopulateDates();
            PopulateFirstSellableModules();
            PopulateEditModes();
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
            PopulateModules(y, m);
            teamName = teamName == null ? "" : teamName;
            //location = location == null ? "" : location;
            type = type == null ? "" : type;
            var model = new MeasurementSearchModel() { EpicId = epicId, Year = y, Month = m, YearMonth = yearMonth, NextMonth = NextMonth, NextYear = NextYear, Location = location, Type = type, TeamName = teamName, MaxYear = maxYear, MaxMonth = maxMonth };
            return View(model);
        }

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

        #endregion

        #region Reads

        public ActionResult EditingInLineDetails_Read([DataSourceRequest] DataSourceRequest request, int year, int month, string location, string isFSM, string team)
        {
            if (location == "All")
            {
                location = null;
            }
            if (isFSM == "All")
            {
                isFSM = null;
            }
            if (team == "All")
            {
                team = null;
            }
            var x = _repository.FillMeasurementDetails(year, month, location, isFSM, team);
            return Json(x.ToDataSourceResult(request));
        }

        public ActionResult EditingInLine_Read([DataSourceRequest] DataSourceRequest request, int epicId, int year, int month, string location, string type, string teamName)
        {
            Team team = null;
            List<Measurement> measurements = new List<Measurement>();
            int id = _repository.GetEmployeeId(UserId);
            if (id != 1001 && id != 1002 && id != 2001 && id != 2002 && id != 1003 && id != 2003)
            {
                Employee employee = _repository.GetEmployeeById(id);
                if (employee.EmployeeType.TypeName == "Team Leader")
                {
                    List<Team> temp = (List<Team>)_repository.GetTeamAll(0, null, employee.EmployeeId, 0);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        team = temp[i];
                        if (team != null)
                        {
                            List<Measurement> t = _repository.SearchMeasurement(year, month, location, type, team.TeamName);
                            measurements.AddRange(t);
                        }
                    }
                }
                else if (employee.EmployeeType.TypeName == "Project Manager" || employee.EmployeeType.TypeName == "Program Manager" || employee.EmployeeType.TypeName == "Admin")
                {
                    measurements = _repository.SearchMeasurement(year, month, location, type, teamName);
                }
            }
            else
            {
                measurements = _repository.SearchMeasurement(year, month, location, type, teamName);
            }
            return Json(measurements.ToDataSourceResult(request));
        }

        #endregion

        #region Creates

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Create([DataSourceRequest] DataSourceRequest request, Measurement measurement)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            Measurement temp;
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
                    measurement.Team.TeamLeader = new TeamLeaderViewModel();
                    measurement.Team.ProjectManager = new ProjectManagerViewModel();
                }
                int id = _repository.GetEmployeeId(UserId);
                if (id == 1001)
                {
                    temp = _repository.InsertMeasurement(measurement, "Test Admin Turkey", ipAddress);
                }
                else if (id == 2001)
                {
                    temp = _repository.InsertMeasurement(measurement, "Test Admin Egypt", ipAddress);
                }
                else
                {
                    temp = _repository.InsertMeasurement(measurement, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
                }

                if (temp != null)
                {
                    newMeasurement = _repository.GetMeasurementAll(measurement.EpicId, measurement.Year, measurement.Month, measurement.Type.TypeName, 0).First();
                }
            }
            return Json(new[] { newMeasurement }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Updates

        [Authorize(Roles = "Admin, Project Manager, Program Manager, Team Leader, Tester")]
        [HttpPost]
        public ActionResult EditingInLine_Update([DataSourceRequest] DataSourceRequest request, Measurement measurement)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            Measurement newMeasurement = new Measurement();
            if (measurement != null && ModelState.IsValid)
            {
                if ((DateTime.Today.Year == measurement.Year && (DateTime.Today.Month - measurement.Month >= 2 || DateTime.Today.Month - measurement.Month <= -2)) || (DateTime.Today.Year - measurement.Year >= 2) || (DateTime.Today.Year - measurement.Year == 1 && measurement.Month >= 2))
                {
                    ModelState.AddModelError("Error", "Check ID");
                    ViewBag.EditErrorMessage = "You can only edit the measurements of previous month!";
                    return null;
                }
                int id = _repository.GetEmployeeId(UserId);
                if (id == 1001)
                {
                    newMeasurement = _repository.UpdateMeasurement(measurement, "Test Admin Turkey", ipAddress);
                    List<Measurement> tempList = (List<Measurement>)_repository.GetMeasurementAll(newMeasurement.EpicId, newMeasurement.Month == 12 ? newMeasurement.Year + 1 : newMeasurement.Year, newMeasurement.Month == 12 ? 1 : newMeasurement.Month + 1, newMeasurement.Type.TypeName, 0);
                    if (tempList != null && tempList.Count() > 0)
                    {
                        Measurement temp = tempList[0];
                        temp.PreviousMonthCumulativeActualEffort = newMeasurement.PreviousMonthCumulativeActualEffort + newMeasurement.ActualEffort;
                        temp = _repository.UpdateMeasurement(temp, "Test Admin Turkey", ipAddress);
                    }
                }
                else if (id == 2001)
                {
                    newMeasurement = _repository.UpdateMeasurement(measurement, "Test Admin Egypt", ipAddress);
                    List<Measurement> tempList = (List<Measurement>)_repository.GetMeasurementAll(newMeasurement.EpicId, newMeasurement.Month == 12 ? newMeasurement.Year + 1 : newMeasurement.Year, newMeasurement.Month == 12 ? 1 : newMeasurement.Month + 1, newMeasurement.Type.TypeName, 0);
                    if (tempList != null && tempList.Count() > 0)
                    {
                        Measurement temp = tempList[0];
                        temp.PreviousMonthCumulativeActualEffort = newMeasurement.PreviousMonthCumulativeActualEffort + newMeasurement.ActualEffort;
                        temp = _repository.UpdateMeasurement(temp, "Test Admin Egypt", ipAddress);
                    }
                }
                else
                {
                    newMeasurement = _repository.UpdateMeasurement(measurement, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
                    List<Measurement> tempList = (List<Measurement>)_repository.GetMeasurementAll(newMeasurement.EpicId, newMeasurement.Month == 12 ? newMeasurement.Year + 1 : newMeasurement.Year, newMeasurement.Month == 12 ? 1 : newMeasurement.Month + 1, newMeasurement.Type.TypeName, 0);
                    if (tempList != null && tempList.Count() > 0)
                    {
                        Measurement temp = tempList[0];
                        temp.PreviousMonthCumulativeActualEffort = newMeasurement.PreviousMonthCumulativeActualEffort + newMeasurement.ActualEffort;
                        temp = _repository.UpdateMeasurement(temp, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
                    }
                }
                //update yapılan measurement ın sonraki tarihlerdeki versiyonlarına da güncelleme yap
                if (newMeasurement.Team.TeamId == 0)
                {
                    newMeasurement.Team.TeamName = "";
                    newMeasurement.Team.TeamLeader = new TeamLeaderViewModel();
                    newMeasurement.Team.ProjectManager = new ProjectManagerViewModel();
                }
            }

            return Json(new[] { newMeasurement }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Deletes

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        [HttpPost]
        public ActionResult EditingInLine_Destroy([DataSourceRequest] DataSourceRequest request, Measurement measurement)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (measurement != null)
            {
                int id = _repository.GetEmployeeId(UserId);
                if (id == 1001)
                {
                    _repository.DeleteMeasurement(measurement.EpicId, measurement.Year, measurement.Month, _repository.GetParameterValue("Type", measurement.Type.TypeName), "Test Admin Turkey", ipAddress);
                }
                else if (id == 2001)
                {
                    _repository.DeleteMeasurement(measurement.EpicId, measurement.Year, measurement.Month, _repository.GetParameterValue("Type", measurement.Type.TypeName), "Test Admin Egypt", ipAddress);
                }
                else
                {
                    _repository.DeleteMeasurement(measurement.EpicId, measurement.Year, measurement.Month, _repository.GetParameterValue("Type", measurement.Type.TypeName), _repository.GetEmployeeById(id).EmployeeName, ipAddress);
                }

            }

            return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Feature

        public ActionResult Feature_Read([DataSourceRequest] DataSourceRequest request, int epicId, int year, int month)
        {
            List<Feature> features = new List<Feature>();
            features = (List<Feature>)_repository.GetFeatureAll(0, 0, epicId, year, month, 2, 0);
            return Json(features.ToDataSourceResult(request));
        }
        public ActionResult FeatureReport_Read([DataSourceRequest] DataSourceRequest request, int epicId, int year, int month)
        {
            List<FeatureReport> features = new List<FeatureReport>();
            features = (List<FeatureReport>)_repository.GetFeatureReport(0, 0, epicId, year, month, 2);
            return Json(features.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Feature_Create([DataSourceRequest] DataSourceRequest request, Feature feature)
        {
            if (feature != null && ModelState.IsValid)
            {
                _repository.InsertFeature(feature);
                CalculateFeature(feature);
            }
            return Json(new[] { feature }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Feature_Update([DataSourceRequest] DataSourceRequest request, Feature feature)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (feature != null && ModelState.IsValid)
            {
                _repository.UpdateFeature(feature, "", ipAddress);
                CalculateFeature(feature);
            }
            return Json(new[] { feature }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Feature_Destroy([DataSourceRequest] DataSourceRequest request, Feature feature)
        {
            if (feature != null && ModelState.IsValid)
            {
                _repository.DeleteFeature(feature);
                CalculateFeature(feature);
            }
            return Json(new[] { feature }.ToDataSourceResult(request, ModelState));
        }

        public void CalculateFeature(Feature feature)
        {
            List<Feature> features = (List<Feature>)_repository.GetFeatureAll(0, 0, feature.EpicId, feature.Year, feature.Month, 2, 0);

            if (features != null && features.Count() > 0)
            {
                List<Measurement> temp = (List<Measurement>)_repository.GetMeasurementAll(feature.EpicId, feature.Year, feature.Month, "Actual", 0);
                Measurement measurement = null;
                if (temp != null && temp.Count() > 0)
                {
                    measurement = temp[0];
                }
                if (measurement != null && measurement.EditMode.Name == "Feature")
                {
                    measurement.RequirementProgress = 0;
                    measurement.DesignProgress = 0;
                    measurement.DevelopmentProgress = 0;
                    measurement.TestProgress = 0;
                    measurement.UatProgress = 0;

                    float totalEstimation = 0;
                    for (int i = 0; i < features.Count(); i++)
                    {
                        totalEstimation += features[i].FeatureEstimation;
                    }
                    for (int i = 0; i < features.Count(); i++)
                    {
                        measurement.RequirementProgress += features[i].RequirementProgress * (features[i].FeatureEstimation / totalEstimation);
                        measurement.DesignProgress += features[i].DesignProgress * (features[i].FeatureEstimation / totalEstimation);
                        measurement.DevelopmentProgress += features[i].DevelopmentProgress * (features[i].FeatureEstimation / totalEstimation);
                        measurement.TestProgress += features[i].TestProgress * (features[i].FeatureEstimation / totalEstimation);
                        measurement.UatProgress += features[i].UatProgress * (features[i].FeatureEstimation / totalEstimation);
                    }
                    _repository.UpdateMeasurement(measurement, "", "");
                }
            }


        }

        #endregion

        #region Operations

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        public IActionResult GenerateNextMonth(int year, int month, string location)
        {
            Date date = new Date() { Year = year, Month = month };
            if (year == 0 || month == 0)
            {
                date = _repository.GetDates()[0];
                if (date.Month == 12)
                {
                    date.Month = 1;
                    date.Year++;
                }
                else
                {
                    date.Month++;
                }
            }
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            int id = _repository.GetEmployeeId(UserId);
            if (id == 1001)
            {
                _repository.GenerateMeasurementForNextMonth(date.Year, date.Month, null, "Test Admin Turkey", ipAddress);
            }
            else if (id == 2001)
            {
                _repository.GenerateMeasurementForNextMonth(date.Year, date.Month, null, "Test Admin Egypt", ipAddress);
            }
            else
            {
                _repository.GenerateMeasurementForNextMonth(date.Year, date.Month, null, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
            }
            _repository.InsertDateControl(new DateControl()
            {
                Year = date.Year,
                Month = date.Month,
                Effort = new EffortViewModel() { EffortId = 2, EffortName = "FALSE" },
                Progress = new ProgressViewModel() { ProgressId = 1, ProgressName = "TRUE" },
                Variance = new VarianceViewModel() { VarianceId = 2, VarianceName = "FALSE" }
            });
            _repository.GenerateNewMonthTeamTracking(year, month);
            _repository.GenerateNewMonthSubEpic(year, month);

            return RedirectToAction("Editing_Inline", "MeasurementGrid");
        }

        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        public IActionResult DeleteLastMonth(int year, int month, string location)
        {
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            int id = _repository.GetEmployeeId(UserId);
            if (id == 1001)
            {
                _repository.DeleteLastMonth(month, year, location, "Test Admin Turkey", ipAddress);
            }
            else if (id == 2001)
            {
                _repository.DeleteLastMonth(month, year, location, "Test Admin Egypt", ipAddress);
            }
            else
            {
                _repository.DeleteLastMonth(month, year, location, _repository.GetEmployeeById(id).EmployeeName, ipAddress);
            }
            Date date = _repository.GetDates()[0];
            _repository.DeleteDateControl(date.Year, date.Month);
            _repository.DeleteTeamTracking(date.Year, date.Month);
            _repository.DeleteLasMonthFeature(date.Year, date.Month);

            return RedirectToAction("Editing_Inline", "MeasurementGrid");
        }

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

        private void PopulateModules(int year, int month)
        {
            IEnumerable<Module> moduleList = new List<Module>();
            moduleList = _repository.GetModuleProgress(year, month, 0, null);
            ViewData["modules"] = moduleList;
            ViewData["defaultModule"] = new Module() { ModuleId = 0, ModuleName = "", Progress = 0, Weight = 0 };
        }

        private void PopulateTeams()
        {
            List<Team> teamList = new List<Team>();
            List<FeatureTeamModel> featureTeams = new List<FeatureTeamModel>();
            List<Team> temp = new List<Team>();
            temp = (List<Team>)_repository.GetTeamAll(0, null, 0, 0);
            teamList.Add(new Team { TeamId = 0, TeamName = "", TeamLeader = new TeamLeaderViewModel(), ProjectManager = new ProjectManagerViewModel() });
            for (int i = 0; i < temp.Count(); i++)
            {
                teamList.Add(temp[i]);
                featureTeams.Add(new FeatureTeamModel() { TeamId = temp[i].TeamId, TeamName = temp[i].TeamName });
            }
            ViewData["teams"] = teamList;
            ViewData["featureTeams"] = featureTeams;
            ViewData["defaultFeatureTeam"] = new FeatureTeamModel() { TeamId = 0, TeamName = "" };
            ViewData["defaultTeam"] = new Team() { TeamId = 0, TeamName = "" };
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

        private void PopulateFirstSellableModules()
        {
            List<FeatureFSMModel> FSMList = new List<FeatureFSMModel>();
            List<Parameter> parameterList;
            parameterList = (List<Parameter>)_repository.GetParameter("IsFirstSellableModule");
            for (int i = 0; i < parameterList.Count(); i++)
            {
                var temp = new FeatureFSMModel();
                temp.FSMName = parameterList[i].ParameterName;
                temp.FSMValue = parameterList[i].ParameterValue;
                FSMList.Add(temp);
            }
            ViewData["FSMs"] = FSMList;
            ViewData["defaultFSM"] = FSMList.First();
        }

        private void PopulateEditModes()
        {
            List<EditModeModel> editModeList = new List<EditModeModel>();
            editModeList.Add(new EditModeModel() { Value = 1, Name = "Epic" });
            editModeList.Add(new EditModeModel() { Value = 2, Name = "Feature" });
            ViewData["editModes"] = editModeList;
            ViewData["defaultEditMode"] = editModeList.First();
        }

        public string IsVarianceShowed(int year, int month)
        {
            List<DateControl> dateControlList = (List<DateControl>)_repository.GetDateControl(year, month, null);
            bool isVarianceShowed = true;
            int idx = 0;
            for (int i = 0; i < dateControlList.Count(); i++)
            {
                if (dateControlList[i].Variance.VarianceName == "TRUE")
                {
                    idx++;
                }
            }
            if (idx != dateControlList.Count())
            {
                isVarianceShowed = false;
            }
            if (isVarianceShowed)
            {
                return "true";
            }
            else
            {
                return "false";
            }
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

        public JsonResult selectTeams(string location)
        {
            TeamList = (List<Team>)_repository.GetTeamAll(0, null, 0, 0);
            List<string> selectList = new List<string>();
            selectList.Add("All");
            for (int i = 0; i < TeamList.Count(); i++)
            {
                if (TeamList[i].TeamLocation == location || location == "All" || location == null)
                {
                    selectList.Add(TeamList[i].TeamName);
                }
            }
            return Json(selectList);
        }

        public JsonResult selectLocations()
        {
            List<string> locations = new List<string>()
            {
                "All", "Turkey", "Egypt"
            };
            return Json(locations);
        }

        public JsonResult selectFSM()
        {
            List<string> locations = new List<string>()
            {
                "All", "Phase-4", "Phase-5", "Phase-6"
            };
            return Json(locations);
        }

        #endregion
    }
}