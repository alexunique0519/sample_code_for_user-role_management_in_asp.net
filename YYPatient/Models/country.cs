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
    
    public partial class country
    {
        public country()
        {
            this.provinces = new HashSet<province>();
        }
    
        public string countryCode { get; set; }
        public string name { get; set; }
        public string postalPattern { get; set; }
        public string phonePattern { get; set; }
    
        public virtual ICollection<province> provinces { get; set; }
    }
}
