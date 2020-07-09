using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic_Project.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace Epic_Project.Controllers
{
    public class FeatureController : Controller
    {
        private readonly IRepository _repository;

        public FeatureController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Editing_InLine()
        {
            var model = new MeasurementSearchModel();
            return View(model);
        }

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
            if (feature != null && ModelState.IsValid)
            {
                _repository.UpdateFeature(feature);
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
                if (measurement != null)
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
                    _repository.UpdateMeasurement(measurement, null, null);
                }
            }
        }



        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    }
}