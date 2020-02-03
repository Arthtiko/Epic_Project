﻿using System;
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

namespace Epic_Project.Controllers
{
    [Authorize(Roles = "Admin, Project Manager, Program Manager")]
    public class FinanceController : Controller
    {
        private IHttpContextAccessor _accessor;
        private readonly IRepository _repository;
        private string UserId;

        #region Constructor

        public FinanceController(IRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
            _repository = repository;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        #endregion

        #region Views

        public ActionResult Finance_Management(int year, int month, string yearMonth)
        {
            var model = new FinanceSearchModel();
            if (yearMonth != null && yearMonth != "")
            {
                year = Convert.ToInt32(yearMonth.Split('-')[0]);
                month = Convert.ToInt32(yearMonth.Split('-')[1]);
            }
            else
            {
                Date date = _repository.GetFinanceDates()[0];
                year = date.Year;
                month = date.Month;
            }
            Employee emp;
            int employeeId = _repository.GetEmployeeId(UserId);
            if (employeeId != 1001 && employeeId != 1002 && employeeId != 2001 && employeeId != 2002)
            {
                emp = _repository.GetEmployeeById(employeeId);
                if (emp.EmployeeLocation.LocationName == "Turkey")
                {
                    model = new FinanceSearchModel()
                    {
                        IsTurkey = true,
                        Year = year,
                        Month = month,
                        YearMonth = yearMonth
                    };
                }
            }
            else
            {
                model = new FinanceSearchModel()
                {
                    IsTurkey = false,
                    Year = year,
                    Month = month,
                    YearMonth = yearMonth
                };
            }
            return View(model);
        }

        public ActionResult Finance_Report(int year, int month, string yearMonth)
        {
            var model = new FinanceSearchModel();
            if (yearMonth != null && yearMonth != "")
            {
                year = Convert.ToInt32(yearMonth.Split('-')[0]);
                month = Convert.ToInt32(yearMonth.Split('-')[1]);
            }
            else
            {
                Date date = _repository.GetFinanceDates()[0];
                year = date.Year;
                month = date.Month;
            }

            List<FinanceReport> reportList = (List<FinanceReport>)_repository.GetFinanceReport(null, year, month);
            float periodActualPercentage = 0;
            float totalActualPercentage = 0;
            float tempTotalBudget = 0;
            float tempPeriodBudget = 0;
            float tempTotalActual = 0;
            for (int i = 0; i < reportList.Count(); i++)
            {
                FinanceReport temp = reportList[i];
                tempTotalActual += temp.Actual;
                tempPeriodBudget += temp.PeriodBudget;
                tempTotalBudget += temp.TotalBudget;
            }
            periodActualPercentage = (float)Math.Round((tempTotalActual / tempPeriodBudget) * 100, 2);
            totalActualPercentage = (float)Math.Round((tempTotalActual / tempTotalBudget) * 100, 2);

            Employee emp;
            int employeeId = _repository.GetEmployeeId(UserId);
            if (employeeId != 1001 && employeeId != 1002 && employeeId != 2001 && employeeId != 2002)
            {
                emp = _repository.GetEmployeeById(employeeId);
                if (emp.EmployeeLocation.LocationName == "Turkey")
                {
                    model = new FinanceSearchModel()
                    {
                        IsTurkey = true,
                        Year = year,
                        Month = month,
                        YearMonth = yearMonth,
                        PeriodActualPercentage = periodActualPercentage,
                        TotalActualPercentage = totalActualPercentage
                    };
                }
            }
            else
            {
                model = new FinanceSearchModel()
                {
                    IsTurkey = false,
                    Year = year,
                    Month = month,
                    YearMonth = yearMonth,
                    PeriodActualPercentage = periodActualPercentage,
                    TotalActualPercentage = totalActualPercentage
                };
            }
            return View(model);
        }

        public ActionResult Finance_Graph()
        {
            return View();
        }

        #endregion

        #region Reads

        public ActionResult Finance_Management_Read([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            return Json(_repository.GetFinanceAll(null, year, month).ToDataSourceResult(request));
        }

        public ActionResult Finance_Report_Read([DataSourceRequest] DataSourceRequest request, int year, int month)
        {
            return Json(_repository.GetFinanceReport(null, year, month).ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetReportingPeriodGraph()
        {
            IEnumerable<FinanceGraph> model = _repository.GetFinanceGraph(false);
            return Json(model);
        }

        [HttpPost]
        public ActionResult GetProgramTotalGraph()
        {
            IEnumerable<FinanceGraph> model = _repository.GetFinanceGraph(true);
            return Json(model);
        }

        #endregion

        #region Updates

        public ActionResult Finance_Management_Update([DataSourceRequest] DataSourceRequest request, Finance finance)
        {
            Finance newFinance = _repository.UpdateFinanceAll(finance);
            return Json(new[] { newFinance }.ToDataSourceResult(request));
        }

        #endregion

        #region Operations

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            return File(fileContents, contentType, fileName);
        }

        public IActionResult GenerateNextMonth()
        {
            _repository.GenerateNewFinanceMonth();

            return RedirectToAction("Finance_Management", "Finance");
        }

        public IActionResult DeleteLastMonth()
        {
            _repository.DeleteLastFinanceMonth();

            return RedirectToAction("Finance_Management", "Finance");
        }

        public JsonResult selectDates()
        {
            List<Date> DateList = _repository.GetFinanceDates();
            List<string> selectList = new List<string>();
            for (int i = 0; i < DateList.Count(); i++)
            {
                selectList.Add(DateList[i].Year.ToString() + "-" + DateList[i].Month.ToString());
            }
            return Json(selectList);
        }

        #endregion
    }
}