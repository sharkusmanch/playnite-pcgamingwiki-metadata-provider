using System;
using Playnite.SDK;
using Playnite.SDK.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace PCGamingWikiMetadata
{
    public class PCGWGame : GenericItemOption
    {

        public int PageID {get; set; }

        private List<Link> links;
        public List<Link> Links  { get { return links; } }
        private IDictionary<string, IList<string>> data;
        public IDictionary<string, IList<string>> Data { get { return data; } }

        private IDictionary<string, IList<string>> sobj;
        public IDictionary<string, IList<string>> Sobj { get {return sobj;} }

        public PCGWGame()
        {
            this.links = new List<Link>();
        }

        public PCGWGame(string name, int pageid) 
        {
            this.Name = name;
            this.PageID = pageid;
            this.links = new List<Link>();
            this.links.Add(PCGamingWikiLink());
        }
        protected Link PCGamingWikiLink()
        {
            string escapedName = Uri.EscapeUriString(this.Name);
            return new Link("PCGamingWiki", $"https://www.pcgamingwiki.com/wiki/{escapedName}");
        }

        public void Update(JObject gameData)
        {
            UpdateDynamic(gameData);
        }

        protected void UpdateDynamic(dynamic gameData)
        {   
            this.data = DataToDictionary(gameData.query.data);

            if (gameData.query.sobj.Count != 1)
            {
                Console.WriteLine($"Got sobj list of size: {gameData.query.sobj.Count}");
            }

            this.sobj = DataToDictionary(gameData.query.sobj[0].data);

            this.Name = this.Sobj["_SKEY"][0];
        }

        private IDictionary<string, IList<string>> DataToDictionary(JArray data)
        {
            IDictionary<string, IList<string>> dataDict = new Dictionary<string, IList<string>>();

            foreach (dynamic attribute in data)
            {
                IList<string> dataItems = new List<string>();
                foreach (dynamic item in attribute.dataitem)
                {
                    dataItems.Add((string)item.item);
                }

                dataDict.Add((string)attribute.property, dataItems);
            }

            return dataDict;
        }
    }
}