using Newtonsoft.Json.Linq;

namespace PCGamingWikiMetadata
{
    public class PCGamingWikiJSONParser
    {
        private PCGWGameController gameController;
        private JObject content;
        public PCGamingWikiJSONParser(JObject content, PCGWGameController gameController)
        {
            this.content = content;
            this.gameController = gameController;
            // this.settings = settings;
        }

        public void ParseGameDataJson()
        {
            JToken playAnywhere = this.content.SelectToken("$.parse.links[?(@.* == 'List of Xbox Play Anywhere games')]");
            gameController.SetXboxPlayAnywhere();
        }

        public string PageHTMLText()
        {
            return this.content["parse"]["text"]["*"].ToString();
        }
    }
}
