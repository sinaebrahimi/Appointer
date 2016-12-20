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
    using System.Text.RegularExpressions;

    public partial class WorkingTime
    {
        public int Id { get; set; }
        public int JobCorpId { get; set; }


        [Display(Name = "تاریخ")]
        [UIHint("DateTime")]
        //[RegularExpression(@"^(13\d{2}|[1-9]\d)/(1[012]|0?[1-9])/([12]\d|3[01]|0?[1-9]) ([01][0-9]|2[0-3]):([0-5]?[0-9])$", ErrorMessage = "تاریخ ثبت را بدرستی وارد کنید")]
        public DateTime myDate { get; set; }
        public List<ClockRangeModel> Range { get; set; }

        public String start { get; set; }
        public String end { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    
        public virtual JobCorp JobCorp { get; set; }
    }
}