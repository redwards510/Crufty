using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crufty;

namespace CruftyWeb.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
                websites = new List<CourtWebsite>();
        }

        public List<CourtWebsite> websites { get; set; }


    }
}