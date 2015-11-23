using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crufty
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    [Serializable]
    public class CourtWebsite
    {


        [BsonId]
        public ObjectId Id { get; set; }

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

}
