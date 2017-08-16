using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Crufty;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CruftyWeb.Helpers
{
    public static class MongoDbService
    {
        public static IMongoCollection<CourtWebsite> GetCourtWebSiteCollection()
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoServer"].ConnectionString);
            var database = client.GetDatabase("CourtCruft");
            return database.GetCollection<CourtWebsite>("CourtWebsites");
        }

        public static CourtWebsite GetCourtWebSite(ObjectId id)
        {
            CourtWebsite cw = new CourtWebsite();
            Task.Run(async () =>
            {
                var filter = Builders<CourtWebsite>.Filter.Eq("Id", id);
                cw = await GetCourtWebSiteCollection().Find(filter).FirstAsync();
            }).Wait();
            return cw;
        }

        public static CourtWebsite GetCourtWebSite(string id)
        {            
            return GetCourtWebSite(ObjectId.Parse(id));
        }

        public static IEnumerable<CourtWebsite> GetCourtWebsites(bool displayAll = false)
        {
            var collection = GetCourtWebSiteCollection();
            List<CourtWebsite> websites = new List<CourtWebsite>();            

            Task.Run(async () =>
            {
                var filter = (displayAll) ? new BsonDocument() : Builders<CourtWebsite>.Filter.Eq("Checked", false);
                websites = await collection.Find(filter).ToListAsync();
            }).Wait();

            return websites;
        }

        /// <summary>
        /// overload to handle a courtwebsite object
        /// </summary>
        /// <param name="courtWebsite"></param>
        public static void InsertNewCourt(CourtWebsite courtWebsite)
        {
            InsertNewCourt(courtWebsite.CourtName, courtWebsite.Url, courtWebsite.CourtKey, courtWebsite.SelectionXPathString);
        }

        public static void InsertNewCourt(string courtName, string url, string courtKey, string xPath)
        {
            var courtWebsite = new CourtWebsite
            {
                Id = ObjectId.GenerateNewId(),
                Url = url,
                CourtName = courtName,
                OldPageHtml = "Never Diffed",
                NewPageHtml = "Never Diffed",
                DiffedHtml = "Never Diffed",
                LastChangedDateTime = DateTime.MinValue,
                LastRunDateTime = DateTime.MinValue,
                SelectionXPathString = xPath,
                Checked = false               
            };

            Task.Run(async () =>
            {
                await GetCourtWebSiteCollection().InsertOneAsync(courtWebsite);
            }).Wait();
        }

        public static void MarkCourtAsChecked(ObjectId id)
        {            
            Task.Run(async () =>
            {
                var filter = Builders<CourtWebsite>.Filter.Eq("Id", id);
                var update = Builders<CourtWebsite>.Update.Set("Checked", true);
                await GetCourtWebSiteCollection().UpdateOneAsync(filter, update);
            }).Wait();            
        }

        public static void UpdateXPath(ObjectId id, string xPath)
        {
            Task.Run(async () =>
            {
                var filter = Builders<CourtWebsite>.Filter.Eq("Id", id);
                var update = Builders<CourtWebsite>.Update.Set("SelectionXPathString", xPath);
                await GetCourtWebSiteCollection().UpdateOneAsync(filter, update);
            }).Wait();
        }
    }
}