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
        private int StartMonth;
        private int StartYear;
        private int NextMonth;
        private int NextYear;
        private static int afterComma = 2;
        private static float TurkeyFSMTotal = 0.403F;
        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        [Authorize]
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
            var model = new MeasurementSearchModel() {Year = y, Month = m, YearMonth = yearMonth, NextMonth = NextMonth, NextYear = NextYear, LastMonth = DateList[0].Month, LastYear = DateList[0].Year};
            return View(model);
        }

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
            if (lastDate.Year == date.Year && lastDate.Month == date.Month)
            {
                model.Variance = null;
            }
            else
            {
                model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            }
            model.Total = _repository.GetEpicWeight("Turkey", null);
            model.Total = (float)Math.Round(model.Total*100, afterComma);

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
            if (lastDate.Year == date.Year && lastDate.Month == date.Month)
            {
                model.Variance = null;
            }
            else
            {
                model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            }
            model.Total = _repository.GetEpicWeight("Egypt", null);
            model.Total = (float)Math.Round(model.Total * 100, afterComma);
            
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
            if (lastDate.Year == date.Year && lastDate.Month == date.Month)
            {
                model.Variance = null;
            }
            else
            {
                model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            }
            model.Total = 100;

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
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, "Turkey", "TRUE");
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            if (lastDate.Year == date.Year && lastDate.Month == date.Month)
            {
                model.Variance = null;
            }
            else
            {
                model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            }
            //model.Total = _repository.GetEpicWeight("Turkey", "TRUE");
            model.Total = TurkeyFSMTotal;
            model.Total = (float)Math.Round(model.Total * 100, afterComma);

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
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, "Egypt", "TRUE");
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            if (lastDate.Year == date.Year && lastDate.Month == date.Month)
            {
                model.Variance = null;
            }
            else
            {
                model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            }
            model.Total = _repository.GetEpicWeight("Egypt", "TRUE");
            model.Total = (float)Math.Round(model.Total * 100, afterComma);

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
            ProgressModel model = _repository.GetProgress(date.Year, date.Month, null, "TRUE");
            model.Completed = (float)Math.Round(model.Completed, afterComma);
            model.ActualEffort = (float)Math.Round(model.ActualEffort, afterComma);
            if (lastDate.Year == date.Year && lastDate.Month == date.Month)
            {
                model.Variance = null;
            }
            else
            {
                model.Variance = (float)Math.Round((float)model.Variance, afterComma);
            }
            //model.Total = _repository.GetEpicWeight(null, "TRUE");
            model.Total = _repository.GetEpicWeight("Egypt", "TRUE");
            model.Total = model.Total + TurkeyFSMTotal;
            model.Total = (float)Math.Round(model.Total * 100, afterComma);

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
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Turkey", null);
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetEgyptLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Egypt", null);
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetTotalLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress(null, null);
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetTurkeyFirstSellableLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Turkey", "TRUE");
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetEgyptFirstSellableLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress("Egypt", "TRUE");
            return Json(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetTotalFirstSellableLineChart()
        {
            IEnumerable<LineChartModel> model = _repository.GetLineChartProgress(null, "TRUE");
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

        [Authorize]
        public ActionResult GetHighLevelTurkeyFSM([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress("Turkey", "TRUE", date);
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
        public ActionResult GetHighLevelEgyptFSM([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress("Egypt", "TRUE", date);
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
        public ActionResult GetHighLevelOverallFSM([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            Date date = new Date()
            {
                Year = year,
                Month = month
            };
            List<HighLevelProgress> model = (List<HighLevelProgress>)_repository.GetHighLevelProgress(null, "TRUE", date);
            model[0].TargetFinishDate = "29/12/2019";
            model[1].TargetFinishDate = "11/03/2020";
            model[2].TargetFinishDate = "02/04/2020";
            model[3].TargetFinishDate = "29/04/2020";
            model[4].TargetFinishDate = "30/04/2020";
            return Json(model.ToDataSourceResult(request));
        }


        #endregion

        [Authorize]
        public JsonResult selectDates()
        {
            List<Date> DateList = new List<Date>();
            List<string> selectList = new List<string>();
            DateList = _repository.GetDates();
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

    }
}
