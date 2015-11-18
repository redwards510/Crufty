using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Crufty;
using CruftyWeb.Models;
using MongoDB.Driver;

namespace CruftyWeb.Controllers
{
    public class ColoredDiffController : Controller
    {        
        public ActionResult Index(string id)
        {
            var model = new ColoredDiffModel {CourtWebsite = Helpers.MongoDbService.GetCourtWebSite(id)};
            return View(model);
        }
    }
}