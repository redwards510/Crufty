using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using ScrapySharp.Html.Forms;
using ScrapySharp.Html;

namespace Crufty
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = @"http://www.alameda.courts.ca.gov/Pages.aspx/Hours";
            //string regex = 
            //var webClient = new System.Net.WebClient();
            //var page = webClient.DownloadString(url);            

            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri(url));

            //PageWebForm form = homePage.FindFormById("sb_form");
            //form["q"] = "scrapysharp";
            //form.Method = HttpVerb.Get;
            //WebPage resultsPage = form.Submit();

            var resultsLinks = homePage.Html.CssSelect(".contentTable").ToList();
            // ignore th rows.
            // get first and second td
            var selGoodRows = homePage.Html.SelectNodes("//table[@class='contentTable']/tbody/tr").Select(
                    r => new
                    {
                        Branch = r.SelectSingleNode(".//td[1]"),
                        Hours = r.SelectSingleNode(".//td[2]")
                    }
                );

            var tds = homePage.Html.SelectNodes("//table[@class='contentTable']/tbody/tr").Where(s => s.ParentNode.ParentNode.InnerHtml.Contains("Alameda") && !s.InnerHtml.Contains("<th"));
            //var td1 = tds.CssSelect("td").Where(x => x.XPath == "/html[1]/body[1]/div[3]/div[1]/div[1]/div[4]/div[2]/table[1]/tr[1]/td[3]/div[2]/table[1]/tbody[1]/tr[3]/td[1]");
            //var td1text = td1.Select(f => f.se)
            var td2 = tds.CssSelect("td").Select(x => 
                new {
                    CourtName = x.SelectSingleNode(".//td[1]"),
                    Hourts = x.SelectSingleNode(".//td[2]")
                }
            );

            //var hours = td2.FirstOrDefault(t => t.XPath.EndsWith("td[2]")).InnerText;
            //var hours = moretd.FirstOrDefault().InnerText;


            WebPage blogPage = homePage.FindLinks(By.Text("romcyber blog | Just another WordPress site")).Single().Click();
        }
    }
}
