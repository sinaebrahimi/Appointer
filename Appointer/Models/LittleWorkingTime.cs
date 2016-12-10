using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Appointer.Models
{
    public class LittleWorkingTime
    {
        public string StartDate { get; set; }
        public string EndTime { get; set; }

        public string StartTime { get; set; }

        public string dow { get; set; }

        public string JobTitle { get; set; }

        public string JobCorp { get; set; }


        public List<Item> ap { get; set; }

        public List<WTItem> wt { get; set; }


    }

    public class WTItem
    {
        public DateTime StartTime { set; get; }
        public DateTime EndTime { set; get; }

        public string StartHour { set; get; }
        public string EndHour { set; get; }

    }

    public class Item
    {
        public DateTime StartTime { set; get; }
        public DateTime EndTime { set; get; }

        public string StartHour { set; get; }
        public string EndHour { set; get; }

    }
}

