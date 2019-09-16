using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class Employee
    {
        public Employee()
        {
            EmployeeType = new EmployeeTypeViewModel();
        }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [UIHint("ClientEmployeeType")]
        public EmployeeTypeViewModel EmployeeType { get; set; }
        public string EmployeeLocation { get; set; }
    }

    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }

    public class EmployeeTypeViewModel
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
