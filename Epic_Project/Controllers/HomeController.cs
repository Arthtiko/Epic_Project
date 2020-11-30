using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epic_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace Epic_Project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository _repository;
        private GraphControl TurkeyOverall = new GraphControl();
        private GraphControl EgyptOverall = new GraphControl();
        private GraphControl TotalOverall = new GraphControl();
        private GraphControl TurkeyFSM = new GraphControl();
        private GraphControl EgyptFSM = new GraphControl();
        private GraphControl TotalFSM = new GraphControl();
        private int StartMonth;
        private int StartYear;
        private int NextMonth;
        private int NextYear;
        private static int afterComma = 2;
        private static float TurkeyFSMTotal = 0.403F;


        #region Constructor

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Views

        public IActionResult Index(int epicId, int year, int month, string yearMonth, string type, string teamName)
        {
            List<Date> DateList = new List<Date>();
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
            List<DateControl> dateControlList = (List<DateControl>)_repository.GetDateControl(y, m, null);
            for (int i = 0; i < dateControlList.Count(); i++)
            {
                if (dateControlList[i].DateControlType.TypeId == 1)
                {
                    if (dateControlList[i].Effort.EffortName == "FALSE")
                    {
                        TurkeyOverall.ShowActualEffort = false;
                    }
                    if (dateControlList[i].Progress.ProgressName == "FALSE")
                    {
                        TurkeyOverall.ShowProgress = false;
                    }
                    if (dateControlList[i].Variance.VarianceName == "FALSE")
                    {
                        TurkeyOverall.ShowVariance = false;
                    }
                }
                else if (dateControlList[i].DateControlType.TypeId == 2)
                {
                    if (dateControlList[i].Effort.EffortName == "FALSE")
                    {
                        TurkeyFSM.ShowActualEffort = false;
                    }
                    if (dateControlList[i].Progress.ProgressName == "FALSE")
                    {
                        TurkeyFSM.ShowProgress = false;
                    }
                    if (dateControlList[i].Variance.VarianceName == "FALSE")
                    {
                        TurkeyFSM.ShowVariance = false;
                    }
                }
                else if (dateControlList[i].DateControlType.TypeId == 3)
                {
                    if (dateControlList[i].Effort.EffortName == "FALSE")
                    {
                        EgyptOverall.ShowActualEffort = false;
                    }
                    if (dateControlList[i].Progress.ProgressName == "FALSE")
                    {
                        EgyptOverall.ShowProgress = false;
                    }
                    if (dateControlList[i].Variance.VarianceName == "FALSE")
                    {
                        EgyptOverall.ShowVariance = false;
                    }
                }
                else if (dateControlList[i].DateControlType.TypeId == 4)
                {
                    if (dateControlList[i].Effort.EffortName == "FALSE")
                    {
                        EgyptFSM.ShowActualEffort = false;
                    }
                    if (dateControlList[i].Progress.ProgressName == "FALSE")
                    {
                        EgyptFSM.ShowProgress = false;
                    }
                    if (dateControlList[i].Variance.VarianceName == "FALSE")
                    {
                        EgyptFSM.ShowVariance = false;
                    }
                }
                else if (dateControlList[i].DateControlType.TypeId == 5)
                {
                    if (dateControlList[i].Effort.EffortName == "FALSE")
                    {
                        TotalOverall.ShowActualEffort = false;
                    }
                    if (dateControlList[i].Progress.ProgressName == "FALSE")
                    {
                        TotalOverall.ShowProgress = false;
                    }
                    if (dateControlList[i].Variance.VarianceName == "FALSE")
                    {
                        TotalOverall.ShowVariance = false;
                    }
                }
                else if (dateControlList[i].DateControlType.TypeId == 6)
                {
                    if (dateControlList[i].Effort.EffortName == "FALSE")
                    {
                        TotalFSM.ShowActualEffort = false;
                    }
                    if (dateControlList[i].Progress.ProgressName == "FALSE")
                    {
                        TotalFSM.ShowProgress = false;
                    }
                    if (dateControlList[i].Variance.VarianceName == "FALSE")
                    {
                        TotalFSM.ShowVariance = false;
                    }
                }
            }
            string tName;
            int teamId;
            if (teamName == null || teamName == "Overall Program")
            {
                teamId = 0;
            }
            else
            {
                tName = teamName.Split('-')[0];
                teamId = Convert.ToInt32(tName);
            }
            var model = new MeasurementSearchModel()
            {
                Year = y,
                Month = m,
                YearMonth = yearMonth,
                NextMonth = NextMonth,
                NextYear = NextYear,
                LastMonth = DateList[0].Month,
                LastYear = DateList[0].Year,
                TeamId = teamId,
                TeamName = "Overall Program",
                TurkeyOverall = TurkeyOverall,
                EgyptOverall = EgyptOverall,
                TotalOverall = TotalOverall,
                TurkeyFSM = TurkeyFSM,
                EgyptFSM = EgyptFSM,
                TotalFSM = TotalFSM
            };
            return View(model);
        }

        #endregion

        #region Graph

        [Authorize]
        [HttpPost]
        public ActionResult GetTurkeyProgress(int year, int month)
        {
            Date date = new Date() { Year = year, Month = month};
            Date lastDate = _repository.GetDates()[0];
            
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, "Turkey", null);
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            model.Total = _repository.GetEpicWeight("Turkey", null);
            model.Total = (float)Math.Round(model.Total*100, afterComma);
            if (model.Completed > model.Total)
            {
                model.Completed = model.Total;
            }

            IEnumerable<ProgressModel> progressList = new ProgressModel[]
            {
                model
            };
            return Json(progressList);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetEgyptProgress(int year, int month)
        {
            Date date = new Date() { Year = year, Month = month };
            Date lastDate = _repository.GetDates()[0];
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, "Egypt", null);
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            model.Total = _repository.GetEpicWeight("Egypt", null);
            model.Total = (float)Math.Round(model.Total * 100, afterComma);
            if (model.Completed > model.Total)
            {
                model.Completed = model.Total;
            }

            IEnumerable<ProgressModel> progressList = new ProgressModel[]
            {
                model
            };
            return Json(progressList);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetOverallProgress(int year, int month)
        {
            Date date = new Date() { Year = year, Month = month };
            Date lastDate = _repository.GetDates()[0];
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, null, null);
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            model.Total = 100;
            if (model.Completed > model.Total)
            {
                model.Completed = model.Total;
            }

            IEnumerable<ProgressModel> progressList = new ProgressModel[]
            {
                model
            };
            return Json(progressList);
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetTurkeyFirstSellableProgress(int year, int month)
        {
            Date date = new Date() { Year = year, Month = month };
            Date lastDate = _repository.GetDates()[0];
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, "Turkey", "Phase-4");
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            model.Total = _repository.GetEpicWeight("Turkey", "Phase-4");
            //model.Total = TurkeyFSMTotal;
            model.Total = (float)Math.Round(model.Total * 100, afterComma);
            if (model.Completed > model.Total)
            {
                model.Completed = model.Total;
            }

            IEnumerable<ProgressModel> progressList = new ProgressModel[]
            {
                model
            };
            return Json(progressList);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetEgyptFirstSellableProgress(int year, int month)
        {
            Date date = new Date() { Year = year, Month = month };
            Date lastDate = _repository.GetDates()[0];
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, "Egypt", "Phase-4");
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            model.Total = _repository.GetEpicWeight("Egypt", "Phase-4");
            model.Total = (float)Math.Round(model.Total * 100, afterComma);
            if (model.Completed > model.Total)
            {
                model.Completed = model.Total;
            }

            IEnumerable<ProgressModel> progressList = new ProgressModel[]
            {
                model
            };
            return Json(progressList);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetOverallFirstSellableProgress(int year, int month)
        {
            Date date = new Date() { Year = year, Month = month };
            Date lastDate = _repository.GetDates()[0];
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, null, "Phase-4");
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            model.Total = _repository.GetEpicWeight(null, "Phase-4");
            //model.Total = _repository.GetEpicWeight("Egypt", "TRUE");
            //model.Total = model.Total + TurkeyFSMTotal;
            model.Total = (float)Math.Round(model.Total * 100, afterComma);
            if (model.Completed > model.Total)
            {
                model.Completed = model.Total;
            }

            IEnumerable<ProgressModel> progressList = new ProgressModel[]
            {
                model
            };
            return Json(progressList);
        }

        #endregion
        
        #region Line Chart
        [Authorize]
        [HttpPost]
        public ActionResult GetTurkeyLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Turkey", null, "Turkey Overall Progress");
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetEgyptLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Egypt", null, "Egypt Overall Progress");
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetTotalLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress(null, null, "Total Overall Progress");
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetTurkeyFirstSellableLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Turkey", "Phase-4", "Turkey FSM Progress");
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetEgyptFirstSellableLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Egypt", "Phase-4", "Egypt FSM Progress");
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetTotalFirstSellableLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress(null, "Phase-4", "Total FSM Progress");
            return Json(model);
        }
        #endregion

        #region High Level Progress
        
        [Authorize]
        public ActionResult GetHighLevelTurkey([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress("Turkey", null, date);
            model[0].TargetFinishDate = "12/12/2019";
            model[1].TargetFinishDate = "24/02/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "09/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }
        public ActionResult GetHighLevelTurkeyNew([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgressNew("Turkey", null, date);
            model[0].TargetFinishDate = "12/12/2019";
            model[1].TargetFinishDate = "24/02/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "09/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }

        [Authorize]
        public ActionResult GetHighLevelTurkeyFSM([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress("Turkey", "Phase-4", date);
            model[0].TargetFinishDate = "12/12/2019";
            model[1].TargetFinishDate = "24/02/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "09/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }
        [Authorize]
        public ActionResult GetHighLevelTurkeyFSMNew([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgressNew("Turkey", "Phase-4", date);
            model[0].TargetFinishDate = "12/12/2019";
            model[1].TargetFinishDate = "24/02/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "09/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }

        [Authorize]
        public ActionResult GetHighLevelEgypt([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress("Egypt", null, date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "31/03/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }
        [Authorize]
        public ActionResult GetHighLevelEgyptNew([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgressNew("Egypt", null, date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "31/03/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }

        [Authorize]
        public ActionResult GetHighLevelEgyptFSM([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress("Egypt", "Phase-4", date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "31/03/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }
        [Authorize]
        public ActionResult GetHighLevelEgyptFSMNew([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgressNew("Egypt", "Phase-4", date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "31/03/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }

        [Authorize]
        public ActionResult GetHighLevelOverall([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress(null, null, date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }
        [Authorize]
        public ActionResult GetHighLevelOverallNew([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgressNew(null, null, date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }

        [Authorize]
        public ActionResult GetHighLevelOverallFSM([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress(null, "Phase-4", date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }
        [Authorize]
        public ActionResult GetHighLevelOverallFSMNew([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgressNew(null, "Phase-4", date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }


        #endregion

        #region Operations

        public JsonResult selectDates()
        {
            List<Date> DateList = new List<Date>();
            List<string> selectList = new List<string>();
            DateList = _repository.GetDates();
            for (int i = 0; i < DateList.Count(); i++)
            {
                selectList.Add(DateList[i].Year.ToString() + "-" + DateList[i].Month.ToString());
            }
            return Json(selectList);
        }

        public JsonResult SelectTeams()
        {
            List<Team> TeamList = new List<Team>();
            TeamList = (List<Team>)_repository.GetTeamAll(0, null, 0, 0);
            List<string> selectList = new List<string>();
            selectList.Add("");
            for (int i = 0; i < TeamList.Count(); i++)
            {
                selectList.Add(TeamList[i].TeamName);
            }
            return Json(selectList);
        }

        #endregion
    }
}
