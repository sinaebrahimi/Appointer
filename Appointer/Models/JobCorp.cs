﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Appointer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class JobCorp
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JobCorp()
        {
            this.Services = new HashSet<Service>();
            this.WorkingTimes = new HashSet<WorkingTime>();
        }
    
        public int Id { get; set; }

        [Display(Name = "نام کاربر")]
        public int UserId { get; set; }

        [Display(Name = "عنوان شغل")]
        public int JobId { get; set; }

        [Display(Name = "سمت اجرابی")]
        public string RoleTitle { get; set; }
    
        public virtual Job Job { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Service> Services { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingTime> WorkingTimes { get; set; }
    }
}
