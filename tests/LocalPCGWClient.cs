using PCGamingWikiMetadata;
using Newtonsoft.Json.Linq;
using System.IO;
public class LocalPCGWClient : PCGWClient
{
    public LocalPCGWClient() { }

    public override void FetchGamePageContent(PCGWGame game)
    {
        JObject content = JObject.Parse(File.ReadAllText($"./data/{game.Name}.json"));

        PCGamingWikiJSONParser jsonParser = new PCGamingWikiJSONParser(content, game);
        PCGamingWikiHTMLParser parser = new PCGamingWikiHTMLParser(jsonParser.PageHTMLText(), game);

        jsonParser.ParseGameDataJson();
        parser.ApplyGameMetadata();
    }
}
