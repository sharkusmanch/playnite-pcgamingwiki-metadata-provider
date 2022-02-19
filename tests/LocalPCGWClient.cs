using PCGamingWikiMetadata;
using Newtonsoft.Json.Linq;
using System.IO;


public class LocalPCGWClient : PCGWClient
{
    public LocalPCGWClient() : base(null)
    {
        this.options = new TestMetadataRequestOptions();
    }

    public LocalPCGWClient(TestMetadataRequestOptions options) : base(null)
    {
        this.options = options;
    }

    public override void FetchGamePageContent(PCGWGame game)
    {
        game.LibraryGame = this.options.GameData;

        JObject content = JObject.Parse(File.ReadAllText($"./data/{game.Name}.json"));

        PCGamingWikiJSONParser jsonParser = new PCGamingWikiJSONParser(content, game);
        PCGamingWikiHTMLParser parser = new PCGamingWikiHTMLParser(jsonParser.PageHTMLText(), game);

        jsonParser.ParseGameDataJson();
        parser.ApplyGameMetadata();
    }
}
