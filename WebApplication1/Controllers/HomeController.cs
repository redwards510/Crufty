using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB;
using MongoDB.Driver;
using Crufty;
using CruftyWeb.Models;

namespace CruftyWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var client = new MongoClient("mongodb://dev-web-ext");
            var database = client.GetDatabase("CourtCruft");
            var collection = database.GetCollection<CourtWebsite>("CourtWebsites");
            var model = new HomeViewModel(); 


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}