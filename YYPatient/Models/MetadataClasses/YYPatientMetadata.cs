using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YYPatient.Models
{
    [MetadataType(typeof(YYPatientMetadata))]
    public partial class patient
    {

    }

    public class YYPatientMetadata
    {
        [Display(Name = "Patient #")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d5}")]
        public int patientId { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

     
        [Display(Name = "Last Name")]
        public string lastName { get; set; }
        
        [Display(Name = "Street Address")]
        public string address { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "Province")]
        public string provinceCode { get; set; }

        [Display(Name = "Postal Code")]
        public string postalCode { get; set; }

        [Display(Name = "OHIP #")]
        public string OHIP { get; set; }

        //define the format for the date string
        [Display(Name = "Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> dateOfBirth { get; set; }

        [Display(Name = "Deceased?")]
        public bool deceased { get; set; }

        [Display(Name = "Date of Death")]
        public Nullable<System.DateTime> dateOfDeath { get; set; }

        [Display(Name = "Home Phone")]
        public string homePhone { get; set; }

        [Display(Name = "Gender")]
        public string gender { get; set; }

    }
}