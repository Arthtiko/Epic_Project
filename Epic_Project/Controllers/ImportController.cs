using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Epic_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epic_Project.Controllers
{
    [Authorize(Roles = "Admin, Project Manager, Program Manager")]
    public class ImportController : Controller
    {
        private IHttpContextAccessor _accessor;
        private readonly IRepository _repository;
        private string UserId;
        public ImportController(IRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
            _repository = repository;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public IActionResult Index(int mode)
        {
            var model = new ImportSearchModel()
            {
                Mode = mode == 0 ? 1 : mode
            };
            return View(model);
        }
        
        [Authorize(Roles = "Admin, Project Manager, Program Manager")]
        public void ImportMeasurement(int epicId, int year, int month, int type, float req, float des, float dev, float test, float uat, float effort, int mode)
        {
            Measurement measurement = new Measurement();
            string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            Date date = new Date() { Year = DateTime.Today.Year, Month = DateTime.Today.Month };
            if ((date.Year == year && date.Month - month <= 1 && date.Month - month >= 0) || (date.Year == year + 1 && month == 12))
            {
                measurement = new Measurement()
                {
                    EpicId = epicId,
                    Year = year,
                    Month = month,
                    Type = new MeasurementTypeViewModel()
                    {
                        TypeName = type == 1 ? "Target" : "Actual",
                        TypeValue = type
                    },
                    RequirementProgress = req,
                    DesignProgress = des,
                    DevelopmentProgress = dev,
                    TestProgress = test,
                    UatProgress = uat,
                    ActualEffort = effort
                };

                measurement = CreateMeasurementForImport(measurement, mode);
                if (measurement != null)
                {
                    int id = _repository.GetEmployeeId(UserId);

                    if (id == 1001)
                    {
                        EpicBaseLine epic = ((List<EpicBaseLine>)_repository.GetEpicBaseLineAll(epicId))[0];

                        if (epic.ProjectLocation.LocationName == "Turkey")
                        {
                            measurement = _repository.UpdateMeasurement(measurement, "Turkey Test Admin", ipAddress);
                        }
                    }
                    else if (id == 2001)
                    {
                        EpicBaseLine epic = ((List<EpicBaseLine>)_repository.GetEpicBaseLineAll(epicId))[0];

                        if (epic.ProjectLocation.LocationName == "Egypt")
                        {
                            measurement = _repository.UpdateMeasurement(measurement, "Egypt Test Admin", ipAddress);
                        }
                    }
                    else
                    {
                        Employee emp = _repository.GetEmployeeById(id);
                        EpicBaseLine epic = ((List<EpicBaseLine>)_repository.GetEpicBaseLineAll(id))[0];

                        if (emp.EmployeeLocation.LocationName == epic.ProjectLocation.LocationName)
                        {
                            measurement = _repository.UpdateMeasurement(measurement, emp.EmployeeName, ipAddress);
                        }
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Index_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public Measurement CreateMeasurementForImport(Measurement measurement, int mode)
        {
            Measurement result = new Measurement();
            measurement = CorrectNumbers(measurement, mode);
            if (measurement == null)
            {
                return null;
            }

            List<Measurement> list = (List<Measurement>)_repository.GetMeasurementAll(measurement.EpicId, measurement.Year, measurement.Month, measurement.Type.TypeName);
            if (list != null && list.Count() > 0)
            {
                result = list[0];
            }
            else
            {
                return null;
            }

            int prevYear = measurement.Month == 1 ? measurement.Year - 1 : measurement.Year;
            int prevMonth = measurement.Month == 1 ? 12 : measurement.Month - 1;
            Measurement prevMonthMeasurement = ((List<Measurement>)_repository.GetMeasurementAll(measurement.EpicId, prevYear, prevMonth, measurement.Type.TypeName))[0];
            measurement.PreviousMonthCumulativeActualEffort = prevMonthMeasurement.PreviousMonthCumulativeActualEffort + prevMonthMeasurement.ActualEffort;

            if (mode == 1)
            {
                measurement.ActualEffort = result.ActualEffort;
            }
            else if (mode == 2)
            {
                measurement.RequirementProgress = result.RequirementProgress;
                measurement.DesignProgress = result.DesignProgress;
                measurement.DevelopmentProgress = result.DevelopmentProgress;
                measurement.TestProgress = result.TestProgress;
                measurement.UatProgress = result.UatProgress;
            }

            return measurement;
        }

        public Measurement CorrectNumbers(Measurement m, int mode)
        {
            if (m.EpicId == 0 || m.Year == 0 || m.Month == 0 || m.Type.TypeValue == 0)
            {
                return null;
            }
            m.RequirementProgress = m.RequirementProgress < 0 ? 0 : m.RequirementProgress > 100 ? 100 : m.RequirementProgress;
            m.DesignProgress = m.DesignProgress < 0 ? 0 : m.DesignProgress > 100 ? 100 : m.DesignProgress;
            m.DevelopmentProgress = m.DevelopmentProgress < 0 ? 0 : m.DevelopmentProgress > 100 ? 100 : m.DevelopmentProgress;
            m.TestProgress = m.TestProgress < 0 ? 0 : m.TestProgress > 100 ? 100 : m.TestProgress;
            m.UatProgress = m.UatProgress < 0 ? 0 : m.UatProgress > 100 ? 100 : m.UatProgress;
            m.ActualEffort = m.ActualEffort < 0 ? 0 : m.ActualEffort > 100 ? 100 : m.ActualEffort;

            return m;
        }

    }
}