//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YYPatient.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class patientMedication
    {
        public int patientMedicationId { get; set; }
        public int patientTreatmentId { get; set; }
        public string din { get; set; }
        public Nullable<double> dose { get; set; }
        public Nullable<int> frequency { get; set; }
        public string frequencyPeriod { get; set; }
        public string exactMinMax { get; set; }
    
        public virtual medication medication { get; set; }
        public virtual patientTreatment patientTreatment { get; set; }
    }
}
