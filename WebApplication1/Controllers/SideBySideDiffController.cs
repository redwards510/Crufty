using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Crufty;
using CruftyWeb.Models;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using MongoDB.Driver;

namespace CruftyWeb.Controllers
{
    public class SideBySideDiffController : Controller
    {
        private readonly ISideBySideDiffBuilder diffBuilder;

        public SideBySideDiffController(ISideBySideDiffBuilder bidiffBuilder)
        {
            diffBuilder = bidiffBuilder;
        }

        public SideBySideDiffController()
        {
            diffBuilder = new SideBySideDiffBuilder(new Differ());
        }

        [ValidateInput(false)]
        public ActionResult Index(string id)
        {
            var cw = Helpers.MongoDbService.GetCourtWebSite(id);
            var model = diffBuilder.BuildDiffModel(cw.OldPageHtml ?? string.Empty, cw.NewPageHtml ?? string.Empty);            
            return View(model);
        }       

    }
}