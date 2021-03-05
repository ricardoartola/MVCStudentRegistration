using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCStudentRegistration.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EnrollmentDate { get; set; }
        public HttpPostedFileBase Photo { get; set; }
        public byte[] PhotoDB { get; set; }
    }
}