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

namespace Epic_Project.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private IHttpContextAccessor _accessor;
        private readonly IRepository _repository;
        private string UserId;
        public ProfileController(IRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
            _repository = repository;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Index_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ProfileModel> model = getProfile();
            return Json(model.ToDataSourceResult(request));
        }
        [Authorize]
        public List<ProfileModel> getProfile()
        {
            List<ProfileModel> model = new List<ProfileModel>();
            int id = _repository.GetEmployeeId(UserId);
            if (id != 1001 && id != 1002 && id != 2001 && id != 2002)
            {
                Employee emp = _repository.GetEmployeeById(id);
                model = new List<ProfileModel>()
                {
                    new ProfileModel()
                    {
                        TitleName = "Employee Id",
                        TitleValue = emp.EmployeeId.ToString()
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Name",
                        TitleValue = emp.EmployeeName
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Type",
                        TitleValue = emp.EmployeeType.TypeName
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Location",
                        TitleValue = emp.EmployeeLocation.LocationName
                    }
                };
            }
            else if (id == 1001 || id == 1002)
            {
                model = new List<ProfileModel>()
                {
                    new ProfileModel()
                    {
                        TitleName = "Employee Id",
                        TitleValue = "0"
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Name",
                        TitleValue = "Test Admin"
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Type",
                        TitleValue = "Admin"
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Location",
                        TitleValue = "Turkey"
                    }
                };
            }
            else if (id == 2001 || id == 2002)
            {
                model = new List<ProfileModel>()
                {
                    new ProfileModel()
                    {
                        TitleName = "Employee Id",
                        TitleValue = "0"
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Name",
                        TitleValue = "Test Admin"
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Type",
                        TitleValue = "Admin"
                    },
                    new ProfileModel()
                    {
                        TitleName = "Employee Location",
                        TitleValue = "Egypt"
                    }
                };
            }
            return model;
        }

        
        public ActionResult GetBackup()
        {
            string command = "sqlcmd -S localhost -U SA -Q \"BACKUP DATABASE [EPICDB] TO DISK = N'/var/opt/mssql/data/EPICDB.bak' WITH NOFORMAT, NOINIT, NAME = 'epicdb-full', SKIP, NOREWIND, NOUNLOAD, STATS = 10\"";
            _repository.GetBackup(command);
            return RedirectToAction("Index");
        }
    }
}