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
    [Authorize(Roles = "Admin, Project Manager, Program Manager, Team Leader")]
    public class VarianceAnalysisController : Controller
    {
        private readonly IRepository _repository;

        #region Constructor

        public VarianceAnalysisController(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Views

        public ActionResult ProducingVarianceAnalysis()
        {
            return View();
        }

        public ActionResult NonProducingVarianceAnalysis()
        {
            return View();
        }

        #endregion

        #region Reads

        public ActionResult ProducingVarianceAnalysis_Read([DataSourceRequest] DataSourceRequest request, int teamId, int year, int month)
        {
            return Json(_repository.GetProducingVarianceAnalysis(teamId, year, month).ToDataSourceResult(request));
        }

        public ActionResult NonProducingVarianceAnalysis_Read([DataSourceRequest] DataSourceRequest request, int teamId, int year, int month)
        {
            return Json(_repository.GetNonProducingVarianceAnalysis(teamId, year, month).ToDataSourceResult(request));
        }

        #endregion

        #region Updates
        
        [HttpPost]
        public ActionResult ProducingVarianceAnalysis_Update([DataSourceRequest] DataSourceRequest request, ProgressProducingVarianceAnalysis varAnalysis)
        {
            NonProgressProducingVarianceAnalysis var = ((List<NonProgressProducingVarianceAnalysis>)_repository.GetNonProducingVarianceAnalysis(varAnalysis.Team.TeamId, varAnalysis.Year, varAnalysis.Month))[0];
            VarianceAnalysis varianceAnalysis = new VarianceAnalysis()
            {
                TeamId = varAnalysis.Team.TeamId,
                Year = varAnalysis.Year,
                Month = varAnalysis.Month,
                TargetProgress = varAnalysis.TargetProgress,
                ProgressProducingResourceCount = varAnalysis.ResourceCount,
                ProgressProducingResourceDayOff = varAnalysis.DayOff,
                NonProgressProducingResourceCount = var.NonProgressProducingResourceCount,
                NonProgressProducingResourceDayOff = var.DayOff
            };

            _repository.UpdateVarianceAnalysis(varianceAnalysis);
            
            return Json(new[] { varianceAnalysis }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult NonProducingVarianceAnalysis_Update([DataSourceRequest] DataSourceRequest request, NonProgressProducingVarianceAnalysis varAnalysis)
        {
            ProgressProducingVarianceAnalysis var = ((List<ProgressProducingVarianceAnalysis>)_repository.GetProducingVarianceAnalysis(varAnalysis.Team.TeamId, varAnalysis.Year, varAnalysis.Month))[0];
            VarianceAnalysis varianceAnalysis = new VarianceAnalysis()
            {
                TeamId = varAnalysis.Team.TeamId,
                Year = varAnalysis.Year,
                Month = varAnalysis.Month,
                TargetProgress = varAnalysis.TargetProgress,
                NonProgressProducingResourceCount = varAnalysis.NonProgressProducingResourceCount,
                NonProgressProducingResourceDayOff = varAnalysis.DayOff,
                ProgressProducingResourceCount = var.ResourceCount,
                ProgressProducingResourceDayOff = var.DayOff
            };

            _repository.UpdateVarianceAnalysis(varianceAnalysis);

            return Json(new[] { varianceAnalysis }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Operations

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public JsonResult selectDates()
        {
            List<Date> DateList = _repository.GetVarianceAnalysisDates();
            List<string> selectList = new List<string>();
            for (int i = 0; i < DateList.Count(); i++)
            {
                selectList.Add(DateList[i].Year.ToString() + "-" + DateList[i].Month.ToString());
            }
            return Json(selectList);
        }

        public IActionResult GenerateNextMonth()
        {
            Date date = _repository.GetVarianceAnalysisDates()[0];
            Date dateNew = new Date()
            {
                Year = date.Month == 12 ? date.Year++ : date.Year,
                Month = date.Month == 12 ? 1 : date.Month + 1
            };
            _repository.InsertVarianceAnalysisDate(dateNew);
            List<ProgressProducingVarianceAnalysis> producing = (List<ProgressProducingVarianceAnalysis>)_repository.GetProducingVarianceAnalysis(0, date.Year, date.Month);
            List<NonProgressProducingVarianceAnalysis> nonProducing = (List<NonProgressProducingVarianceAnalysis>)_repository.GetNonProducingVarianceAnalysis(0, date.Year, date.Month);
            for (int i = 0; i < producing.Count(); i++)
            {
                VarianceAnalysis var = new VarianceAnalysis()
                {
                    TeamId = producing[i].Team.TeamId,
                    Year = dateNew.Year,
                    Month = dateNew.Month,
                    TargetProgress = producing[i].TargetProgress,
                    ProgressProducingResourceCount = producing[i].ResourceCount,
                    ProgressProducingResourceDayOff = producing[i].DayOff,
                    NonProgressProducingResourceCount = nonProducing[i].NonProgressProducingResourceCount,
                    NonProgressProducingResourceDayOff = nonProducing[i].DayOff
                };
                _repository.InsertVarianceAnalysis(var);
            }

            return RedirectToAction("ProducingVarianceAnalysis", "VarianceAnalysis");
        }

        public IActionResult DeleteLastMonth()
        {
            Date date = _repository.GetVarianceAnalysisDates()[0];
            List<ProgressProducingVarianceAnalysis> producing = (List<ProgressProducingVarianceAnalysis>)_repository.GetProducingVarianceAnalysis(0, date.Year, date.Month);
            for (int i = 0; i < producing.Count(); i++)
            {
                _repository.DeleteVarianceAnalysis(producing[i].Team.TeamId, date.Year, date.Month);
            }
            _repository.DeleteVarianceAnalysisDate(date);
            return RedirectToAction("ProducingVarianceAnalysis", "VarianceAnalysis");
        }
        #endregion

    }
}