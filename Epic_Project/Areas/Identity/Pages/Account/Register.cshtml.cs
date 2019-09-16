using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Epic_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace Epic_Project.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly string connectionStringEpic = "server=(localdb)\\MSSQLLocalDB;database=EPICDB;Trusted_Connection=True";
        private readonly string connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-Epic_Project-5699E43C-A911-4672-9DD1-E6F31459C896;Trusted_Connection=True;MultipleActiveResultSets=true";

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Register as Admin")]
            public bool IsSavedAsAdmin { get; set; }
            public int EmployeeId { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }
        public List<Employee> GetEmployeeAll()
        {
            List<Employee> EmployeeList = new List<Employee>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionStringEpic))
            {
                string procName = "[sel_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Employee temp = new Employee();
                temp.EmployeeId = Convert.ToInt32(dt.Rows[i]["EmployeeId"]);
                temp.EmployeeName = Convert.ToString(dt.Rows[i]["EmployeeName"]);
                EmployeeList.Add(temp);
            }
            return EmployeeList;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            List<Employee> EmployeeList;
            List<int> empIdList = new List<int>();
            EmployeeList = GetEmployeeAll();
            
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                Employee x = EmployeeList.Find(emp => emp.EmployeeId == Input.EmployeeId);
                if (x == null)
                {
                    Console.Write("Employee could not found.");
                    return null;
                }
                else
                {
                    DataTable dt = new DataTable();
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        string procName = "[sel_EmployeeId]";
                        using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                        {
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlConnection.Open();
                            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                            {
                                sqlDataAdapter.Fill(dt);
                            }
                        }
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Employee temp = new Employee();
                        temp.EmployeeId = Convert.ToInt32(dt.Rows[i]["EmployeeId"]);
                        //temp.EmployeeName = Convert.ToString(dt.Rows[i]["EmployeeName"]);
                        empIdList.Add(temp.EmployeeId);
                    }
                    int tempId = empIdList.Find(id => id == Input.EmployeeId);
                    if (tempId > 0)
                    {
                        Console.Write("Employee already has an account.");
                        return null;
                    }
                }
                //var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var user = new ApplicationUser {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    EmployeeId = Input.EmployeeId
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (Input.IsSavedAsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
