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
    using System.Collections.Generic;
    
    public partial class Task
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Task()
        {
            this.Comment = new HashSet<Comment>();
        }
    
        public int ProjectName { get; set; }
        public int TaskType { get; set; }
        public string Summary { get; set; }
        public int Priority { get; set; }
        public System.DateTime SDate { get; set; }
        public System.DateTime EDate { get; set; }
        public System.Guid UserID { get; set; }
        public Nullable<int> Score { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public int TaskID { get; set; }
        public System.Guid Manager { get; set; }
        public string Tag { get; set; }
        public bool Status { get; set; }
        public Nullable<System.DateTime> DDate { get; set; }
        public bool Completed { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
    
        public virtual TaskPriority TaskPriority { get; set; }
        public virtual TaskType TaskType1 { get; set; }
        public virtual TUser TUser { get; set; }
        public virtual TAdmin TAdmin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual Project Project { get; set; }
    }
}
