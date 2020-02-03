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
    [Authorize(Roles = "Admin, Project Manager, Program Manager")]
    public class LineChartPanelController : Controller
    {
        private readonly IRepository _repository;

        #region Constructor

        public LineChartPanelController(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Views

        public IActionResult Index()
        {
            PopulateDateControlTypes();
            PopulateEfforts();
            PopulateProgresses();
            PopulateVariances();
            return View();
        }

        #endregion

        #region Reads

        public ActionResult DateControl_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_repository.GetDateControl(null, null, null).ToDataSourceResult(request));
        }

        #endregion

        #region Updates

        [HttpPost]
        public ActionResult DateControl_Update([DataSourceRequest] DataSourceRequest request, DateControl dateControl)
        {
            if (dateControl != null && ModelState.IsValid)
            {
                _repository.UpdateDateControl(dateControl);
            }
            return Json(new[] { dateControl }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Operations

        private void PopulateDateControlTypes()
        {
            List<DateControlTypeViewModel> typeList = new List<DateControlTypeViewModel>();
            List<Parameter> parameters = (List<Parameter>)_repository.GetParameter("DateControlType");
            for (int i = 0; i < parameters.Count(); i++)
            {
                typeList.Add(new DateControlTypeViewModel() { TypeId = parameters[i].ParameterValue, TypeName = parameters[i].ParameterName });
            }
            ViewData["dateControlTypes"] = typeList;
            ViewData["defaultDateControlType"] = typeList.First();
        }

        private void PopulateEfforts()
        {
            List<EffortViewModel> effortList = new List<EffortViewModel>();
            List<Parameter> parameters = (List<Parameter>)_repository.GetParameter("IsVisible");
            for (int i = 0; i < parameters.Count(); i++)
            {
                effortList.Add(new EffortViewModel() { EffortId = parameters[i].ParameterValue, EffortName = parameters[i].ParameterName });
            }
            ViewData["efforts"] = effortList;
            ViewData["defaultEffort"] = effortList.First();
        }

        private void PopulateProgresses()
        {
            List<ProgressViewModel> progressList = new List<ProgressViewModel>();
            List<Parameter> parameters = (List<Parameter>)_repository.GetParameter("IsVisible");
            for (int i = 0; i < parameters.Count(); i++)
            {
                progressList.Add(new ProgressViewModel() { ProgressId = parameters[i].ParameterValue, ProgressName = parameters[i].ParameterName });
            }
            ViewData["progresses"] = progressList;
            ViewData["defaultProgress"] = progressList.First();
        }

        private void PopulateVariances()
        {
            List<VarianceViewModel> varianceList = new List<VarianceViewModel>();
            List<Parameter> parameters = (List<Parameter>)_repository.GetParameter("IsVisible");
            for (int i = 0; i < parameters.Count(); i++)
            {
                varianceList.Add(new VarianceViewModel() { VarianceId = parameters[i].ParameterValue, VarianceName = parameters[i].ParameterName });
            }
            ViewData["variances"] = varianceList;
            ViewData["defaultVariance"] = varianceList.First();
        }

        #endregion
    }
}