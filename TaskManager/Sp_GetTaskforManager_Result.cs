//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskManager
{
    using System;
    
    public partial class Sp_GetTaskforManager_Result
    {
        public string ProjectName { get; set; }
        public string Tag { get; set; }
        public int Priority { get; set; }
        public System.Guid Manager { get; set; }
        public System.DateTime EDate { get; set; }
        public bool Status { get; set; }
    }
}
