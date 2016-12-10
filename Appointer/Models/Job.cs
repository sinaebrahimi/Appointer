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

    public partial class Job
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Job()
        {
            this.JobCorps = new HashSet<JobCorp>();
        }
    
        public int Id { get; set; }


        [Required(ErrorMessage = "وارد کردن عنوان شغل الزامی است")]
        [Display(Name = "عنوان شغل")]
        public string Title { get; set; }



        [DataType(DataType.PhoneNumber)]
        [Display(Name = "شماره تلفن")]
        [Required(ErrorMessage = "وارد کردن شماره موبایل الزامی است")]
        [StringLength(11)]
        public string JobPhone { get; set; }


        [Display(Name = "آدرس")]
        public string Address { get; set; }


        [Display(Name = "توضیحات")]
        public string About { get; set; }


        [Display(Name = "نوع شغل")]
        public int JobTypeId { get; set; }


        [Display(Name = "صاحب کار")]
        public int JobOwnerId { get; set; }


        [Display(Name = "کلید ثبت نام برای همکاران")]
        public string EnrollmentKey { get; set; }

        [Display(Name = "شهر")]
        public int CityId { get; set; }
    
        public virtual City City { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobCorp> JobCorps { get; set; }
        public virtual User User { get; set; }
        public virtual JobType JobType { get; set; }
    }
}
