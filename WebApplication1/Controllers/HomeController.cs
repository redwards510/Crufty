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
        public ActionResult Index()
        {
            var model = new HomeViewModel();
            model.websites = Helpers.MongoDbService.GetCourtWebsites(false).ToList();
            return View(model);
        }
    }
}