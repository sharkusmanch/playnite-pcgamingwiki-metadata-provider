using Newtonsoft.Json.Linq;

namespace PCGamingWikiMetadata
{
    public class PCGamingWikiJSONParser
    {
        private PCGWGame game;
        private JObject content;
        private PCGamingWikiMetadataSettings settings;

        public PCGamingWikiJSONParser(JObject content, PCGWGame game, PCGamingWikiMetadataSettings settings)
        {
            this.content = content;
            this.game = game;
            this.settings = settings;
        }

        public void ParseGameDataJson()
        {
            // Limitation: engine tag will only be added if there's a corresponding link
            // JToken engine = this.content.SelectToken("$.parse.links[?(@.ns == 404)]");

            // if (engine != null)
            // {
            //     game.AddTag(engine["*"].ToString().Split(':')[1]);
            // }

            JToken playAnywhere = this.content.SelectToken("$.parse.links[?(@.* == 'List of Xbox Play Anywhere games')]");

            if (playAnywhere != null && this.settings.ImportXboxPlayAnywhere)
            {
                game.SetXboxPlayAnywhere();
            }
        }

        public string PageHTMLText()
        {
            return this.content["parse"]["text"]["*"].ToString();
        }
    }
}
