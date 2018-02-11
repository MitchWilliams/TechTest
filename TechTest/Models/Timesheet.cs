using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechTest.Models
{
  
    public enum PlacementType { Weekly, Monthly }

    //Class that holds all information required to generate timesheets
    public class Timesheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int timesheetId { get; set; }

        [Display(Name = "Reference")]
        public string timesheetTitle { get; set; }

        [Display(Name = "Candidate Name")]
        public string candidateName { get; set; }

        [Display(Name = "Client Name")]
        public string clientName { get; set; }

        [Display(Name = "Job Title")]
        public string jobTitle { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }

        [EnumDataType(typeof(PlacementType), ErrorMessage = "Submitted value is not valid.")]
        [Display(Name = "Placement Type")]
        public PlacementType placementType { get; set; }

        [Display(Name = "Job ID")]
        public int timesheetJob { get; set; }

    }
}