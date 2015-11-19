using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Crufty;

namespace CruftyWeb.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Websites = new List<CourtWebsite>();            
        }

        public bool DisplayAll { get; set; }

        public List<CourtWebsite> Websites { get; set; }
        [DisplayName("Court Name")]
        public string CourtName { get; set; }
        [DisplayName("Url")]
        public string Url { get; set; }
        [DisplayName("XPath to Content")]
        public string XPath { get; set; }
        [DisplayName("Court Key")]
        public string CourtKey { get; set; }


    }
}