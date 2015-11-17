using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace Crufty
{
    public class CourtWebsite
    {
        [BsonId]
        public Guid Id { get; set; }

        public string CourtName { get; set; }
        public string CourtKey { get; set; }

        public string Url { get; set; }
        public string OldPageHtml { get; set; }
        public string NewPageHtml { get; set; }

        public string DiffedHtml { get; set; }

        public DateTime LastChangedDateTime { get; set; }

        public string SelectionXPathString { get; set; }

        public bool Checked { get; set; }

        public DateTime LastRunDateTime { get; set; }
        public string AddedWords { get; set; }
        public string RemovedWords { get; set; }
    }



    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://dev-web-ext");
            var database = client.GetDatabase("CourtCruft");
            var collection = database.GetCollection<CourtWebsite>("CourtWebsites");

            //var courtWebsite = new CourtWebsite
            //{
            //    Id = Guid.NewGuid(),
            //    Url = @"http://www.alameda.courts.ca.gov/Pages.aspx/Hours",
            //    CourtName = "Alameda",
            //    OldPageHtml = string.Empty,
            //    NewPageHtml = string.Empty,
            //    DiffedHtml = string.Empty,
            //    LastChangedDateTime = DateTime.MinValue,
            //    SelectionXPathString = @"//td[@class='rightColumn']"
            //};


            try
            {
                Task.Run(async () =>
                {
                    var documents = await collection.Find(new BsonDocument()).ToListAsync();
                    foreach (var courtWebsite in documents)
                    {
                        //var insertTask = InsertRecord(courtWebsite, collection);
                        //Task.WaitAll(insertTask);
                        //Task.Run(async () =>
                        //{
                        //    await collection.InsertOneAsync(courtWebsite);
                        //}).Wait();


                        // wrap in error handling so we catch 404s and "network down"
                        ScrapingBrowser browser = new ScrapingBrowser();
                        WebPage homePage = browser.NavigateToPage(new Uri(courtWebsite.Url));

                        //PageWebForm form = homePage.FindFormById("sb_form");
                        //form["q"] = "scrapysharp";
                        //form.Method = HttpVerb.Get;
                        //WebPage resultsPage = form.Submit();

                        // grabs the entire middle "Hours" list of tables
                        var currentPage = homePage.Html.SelectSingleNode(courtWebsite.SelectionXPathString).InnerHtml;
                        if (currentPage == null)
                        {
                            //abort with error
                        }

                        
                        if (courtWebsite.NewPageHtml != currentPage)
                        {
                            HtmlDiff diffHelper = new HtmlDiff(courtWebsite.OldPageHtml, courtWebsite.NewPageHtml);
                            // add this style info in or we won't see the highlighted changes! 
                            courtWebsite.DiffedHtml = diffHelper.Build().Insert(0, @"<style>ins {background-color: #cfc;text-decoration: none;} del {    color: #999;    background-color:#FEC8C8;</style>");                            
                            courtWebsite.OldPageHtml = courtWebsite.NewPageHtml;
                            courtWebsite.NewPageHtml = currentPage;
                            courtWebsite.LastChangedDateTime = DateTime.Now;
                            courtWebsite.Checked = false;
                        }

                        courtWebsite.LastRunDateTime = DateTime.Now;

                        var filter = Builders<CourtWebsite>.Filter.Eq(s => s.Id, courtWebsite.Id);
                        Task.Run(async () =>
                        {
                            await collection.ReplaceOneAsync(filter, courtWebsite);
                        }).Wait();
                    }
                }).Wait();
            }
            catch (Exception exception)
            {
                
            }
        }
    }
}

