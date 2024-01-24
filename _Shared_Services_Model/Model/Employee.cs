using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Employee_Shared_Service_Model.Model
{
    public class Employee
    {
        [StringLength(20), Required]
        public string Name { get; set; }
        public int ID { get; set; }
        [StringLength(300)]
        public string Address { get; set; }
        [StringLength(7)]
        public string Role { get; set; }
        [StringLength(15)]
        public string Department { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string Email { get; set; }
        public List<string> SkillSets { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DOB { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DOJ { get; set; }
        public bool IsActive { get; set; }
    }
    public class EmployeeProfile
    {
        public EmployeeProfile()
        {
            profileFile = new List<IFormFile>();
        }
        public List<IFormFile> profileFile { get; set; }
    }

    public class EmployeeProfileInfo
    {
        public int ID { get; set; }
        public int EmpID { get; set; }
        public string fileName { get; set; }
        public string Description { get; set; }
        public IFormFile profile { get; set; }

    }
}
