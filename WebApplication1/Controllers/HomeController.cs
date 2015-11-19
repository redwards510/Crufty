using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB;
using MongoDB.Driver;
using Crufty;
using CruftyWeb.Models;
using MongoDB.Bson;

namespace CruftyWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string displayAll)
        {
            bool showAll = (displayAll != null) && Convert.ToBoolean(displayAll);
            var model = new HomeViewModel();
            model.Websites = Helpers.MongoDbService.GetCourtWebsites(showAll).ToList();
            model.DisplayAll = showAll;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewSite(HomeViewModel model)
        {
            Helpers.MongoDbService.InsertNewCourt(model.CourtName, model.Url, model.CourtKey, model.XPath);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Mark(CourtWebsite model)
        {
            Helpers.MongoDbService.MarkCourtAsChecked(model.Id);
            return RedirectToAction("Index");
        }
    }
}