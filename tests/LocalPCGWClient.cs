using PCGamingWikiMetadata;
using Newtonsoft.Json.Linq;
using System.IO;


public class LocalPCGWClient : PCGWClient
{
    public LocalPCGWClient() : base(null, null)
    {
        this.options = new TestMetadataRequestOptions();
        this.settings = new PCGamingWikiMetadata.PCGamingWikiMetadataSettings();
    }

    public LocalPCGWClient(TestMetadataRequestOptions options) : base(null, null)
    {
        this.options = options;
        this.settings = new PCGamingWikiMetadata.PCGamingWikiMetadataSettings();
    }

    public PCGamingWikiMetadataSettings GetSettings()
    {
        return this.settings;
    }

    public override void FetchGamePageContent(PCGWGame game)
    {
        game.LibraryGame = this.options.GameData;

        JObject content = JObject.Parse(File.ReadAllText($"./data/{game.Name}.json"));

        PCGamingWikiJSONParser jsonParser = new PCGamingWikiJSONParser(content, game, this.settings);
        PCGamingWikiHTMLParser parser = new PCGamingWikiHTMLParser(jsonParser.PageHTMLText(), game, this.settings);

        jsonParser.ParseGameDataJson();
        parser.ApplyGameMetadata();
    }
}
