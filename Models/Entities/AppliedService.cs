//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LaboratoryAppMVVM.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AppliedService
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public double Result { get; set; }
        public System.DateTime FinishedDateTime { get; set; }
        public bool IsAccepted { get; set; }
        public int StatusId { get; set; }
        public int AnalyzerId { get; set; }
        public int UserId { get; set; }
    }
}
