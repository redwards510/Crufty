using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Crufty;
using MongoDB.Driver;

namespace CruftyWeb.Helpers
{
    public static class MongoDbService
    {
        public static IMongoCollection<CourtWebsite> GetCourtWebSiteCollection()
        {
            var client = new MongoClient("mongodb://dev-web-ext");
            var database = client.GetDatabase("CourtCruft");
            return database.GetCollection<CourtWebsite>("CourtWebsites");
        }

        public static CourtWebsite GetCourtWebSite(Guid Id)
        {
            CourtWebsite cw = new CourtWebsite();
            Task.Run(async () =>
            {
                var filter = Builders<CourtWebsite>.Filter.Eq("Id", Id);
                cw = await GetCourtWebSiteCollection().Find(filter).FirstAsync();
            }).Wait();
            return cw;
        }

        public static CourtWebsite GetCourtWebSite(string Id)
        {
            Guid guidId = Guid.Parse(Id);
            return GetCourtWebSite(guidId);
        }

        public static IEnumerable<CourtWebsite> GetCourtWebsites(bool checkedOnly = false)
        {
            var collection = GetCourtWebSiteCollection();
            List<CourtWebsite> websites = new List<CourtWebsite>();
            Task.Run(async () =>
            {
                var filter = Builders<CourtWebsite>.Filter.Eq("Checked", checkedOnly);
                websites = await collection.Find(filter).ToListAsync();
            }).Wait();

            return websites;
        }
    }
}