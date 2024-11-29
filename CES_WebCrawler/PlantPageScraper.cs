using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace CES_WebCrawler
{
    internal static class PlantPageScraper
    {
        public static PlantPage Scrape(string url)
        {
            Console.WriteLine("Loading page: " + url);
            HtmlDocument page = GetPage(url);
            Console.WriteLine("Parsing plant detail boxes...");
            HtmlNode bricksnode = GetBricksNode(page);
            PlantPage pp = new PlantPage();
            pp.PlantName = url.TrimEnd('/').Split("/").Last<string>();
            pp.SourceURL = new Uri(url);
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> brick in GetBricks(bricksnode))
            {
                PlantDetailCategory pdc = new PlantDetailCategory();
                pdc.CategoryName = brick.Key;
                foreach (KeyValuePair<string, List<string>> brickEntry in brick.Value)
                {
                    PlantDetailEntry pde = new PlantDetailEntry();
                    pde.AttributeName = brickEntry.Key;
                    pde.AttributeValues = brickEntry.Value;
                    pdc.CategoryEntries.Add(pde);
                }
                pp.PlantDetails.Add(pdc);
            }
            Console.WriteLine($"Details loaded for \'{pp.PlantName}\'");
            return pp;
        }

        private static HtmlDocument GetPage(string url)
        {
            return new HtmlWeb().Load(url);
        }

        private static HtmlNode GetBricksNode(HtmlDocument doc)
        {
            HtmlNode bricksNode = doc.DocumentNode.SelectNodes("//div").Where(new Func<HtmlNode, bool>(x => x.HasClass("bricks"))).First();
            if (bricksNode == null)
                throw new Exception("no bricks node found on page");
            return bricksNode;
        }
        
        private static Dictionary<string, Dictionary<string, List<string>>> GetBricks(HtmlNode bricksNode)
        {
            Dictionary<string, Dictionary<string, List<string>>> brickInfoCollection = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (HtmlNode brickNode in bricksNode.ChildNodes.Where(new Func<HtmlNode, bool>(x => x.Name == "ul")))
            {
                HtmlNode dl = brickNode.ChildNodes.
                    Where(new Func<HtmlNode, bool>(x => x.Name == "li")).
                    Single<HtmlNode>().ChildNodes.
                    Where(new Func<HtmlNode, bool>(x => x.Name == "dl")).
                    Single<HtmlNode>();
                string attCategoryName = "";
                string currAttEntryName = "";
                Dictionary<string, List<string>> attEntries = new Dictionary<string, List<string>>();
                foreach (HtmlNode hn in dl.ChildNodes)
                {
                    switch (hn.Name)
                    {
                        case ("dt"):
                            currAttEntryName = Helpers.CleanupString(hn.InnerText);
                            if (!attEntries.ContainsKey(currAttEntryName)) attEntries.Add(currAttEntryName, new List<string>());
                            break;
                        case ("dd"):
                            attEntries[currAttEntryName].Add(Helpers.CleanupString(hn.InnerText));
                            break;
                        case ("span"):
                            attCategoryName = Helpers.CleanupString(hn.InnerText);
                            break;
                    }
                }
                brickInfoCollection.Add(attCategoryName, attEntries);
            }
            return brickInfoCollection;
        }
    }
}
