using Playnite.SDK;
using RestSharp;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PCGamingWikiMetadata
{
    public class PCGamingWikiJSONParser
    {
        private PCGWGame game;
        private JObject content;
        public PCGamingWikiJSONParser(JObject content, PCGWGame game)
        {
            this.content = content;
            this.game = game;
        }

        public void ParseGameDataJson()
        {
            // Limitation: engine tag will only be added if there's a corresponding link
            JToken engine = this.content.SelectToken("$.parse.links[?(@.ns == 404)]");

            if (engine != null)
            {
                game.AddTag(engine["*"].ToString().Split(':')[1]);
            }
        }

        public string PageHTMLText()
        {
            return this.content["parse"]["text"]["*"].ToString();
        }
    }
}
