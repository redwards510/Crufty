using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using HtmlAgilityPack;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using ScrapySharp.Html.Forms;
using ScrapySharp.Html;
using Crufty;
using Serilog;

namespace Crufty
{
    public class CruftyConsole
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();
            var log = Log.Logger.ForContext<Crufty.CruftyConsole>();

            // get list of all courts to process
            var client = new MongoClient("mongodb://dev-web-ext");
            var database = client.GetDatabase("CourtCruft");
            var collection = database.GetCollection<CourtWebsite>("CourtWebsites");
            List<CourtWebsite> documents = new List<CourtWebsite>();
            Task.Run(async () =>
            {
                documents = await collection.Find(new BsonDocument()).ToListAsync();
            }).Wait();            
            
            // loop over list of courts and scrape them. 
            foreach (var courtWebsite in documents)
            {
                try
                {
                    // wrap in error handling so we catch 404s and "network down"
                    ScrapingBrowser browser = new ScrapingBrowser();
                    WebPage homePage = browser.NavigateToPage(new Uri(courtWebsite.Url));

                    //PageWebForm form = homePage.FindFormById("sb_form");
                    //form["q"] = "scrapysharp";
                    //form.Method = HttpVerb.Get;
                    //WebPage resultsPage = form.Submit();

                    // if no xpath filter, just grab the whole page
                    var currentPage = (String.IsNullOrEmpty(courtWebsite.SelectionXPathString)) ?
                        homePage.Html.InnerHtml
                        :homePage.Html.SelectSingleNode(courtWebsite.SelectionXPathString).InnerHtml;

                    if (String.IsNullOrWhiteSpace(currentPage))
                    {
                        log.With("CourtWebsite", courtWebsite)
                            .Error("Unable to find {XPath} in {Url}", courtWebsite.SelectionXPathString, courtWebsite.Url);
                        continue;
                    }

                    if (courtWebsite.NewPageHtml != currentPage)
                    {
                        HtmlDiff diffHelper = new HtmlDiff(courtWebsite.NewPageHtml, currentPage);
                        courtWebsite.DiffedHtml = diffHelper.Build().Insert(0, "<style>ins {background-color: #cfc;text-decoration: none;} del {    color: #999;    background-color:#FEC8C8;</style>");
                        courtWebsite.OldPageHtml = courtWebsite.NewPageHtml;
                        courtWebsite.NewPageHtml = currentPage;
                        courtWebsite.LastChangedDateTime = DateTime.Now;
                        courtWebsite.Checked = false;

                        log.With("CourtWebsite", courtWebsite)
                            .Information("Changes found for {CourtName}", courtWebsite.CourtName);
                    }

                    courtWebsite.LastRunDateTime = DateTime.Now;

                    var filter = Builders<CourtWebsite>.Filter.Eq(s => s.Id, courtWebsite.Id);
                    Task.Run(async () =>
                    {
                        await collection.ReplaceOneAsync(filter, courtWebsite);
                    }).Wait();

                    log.With("CourtWebsite", courtWebsite)
                            .Information("Successfully scraped site {CourtName}", courtWebsite.CourtName);
                }
                catch (WebException exception)
                {
                    log.With("CourtWebsite", courtWebsite )
                        .Error(exception, "Web Exception {ExceptionMessage} (check the url) {url}", exception.Message, courtWebsite.Url);
                }
                catch (Exception exception)
                {
                    log.With("CourtWebsite", courtWebsite)
                        .Error(exception, "General Exception {ExceptionMessage}", exception.Message);
                }
            }
        } // end main
    }

    /// <summary>
    /// Example Usage:
    // log.With("User", user)
    // .InformationEvent("NewUser", "{Email}", user.email)
    /// </summary>
    public static class CruftyExtensions
    {
        public static ILogger With(this ILogger logger, string propertyName, object value)
        {
            return logger.ForContext(propertyName, value, true);
        }
    }
}

