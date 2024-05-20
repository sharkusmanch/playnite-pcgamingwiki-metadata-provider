using PCGamingWikiMetadata;
using Newtonsoft.Json.Linq;
using System.IO;

public class LocalPCGWClient : PCGWClient
{
    public LocalPCGWClient() : base(null, null)
    {
        this.options = new TestMetadataRequestOptions();
        PCGamingWikiMetadataSettings settings = new PCGamingWikiMetadata.PCGamingWikiMetadataSettings();
        this.gameController = new PCGWGameController(settings);
    }

    public LocalPCGWClient(TestMetadataRequestOptions options) : base(null, null)
    {
        this.options = options;
        PCGamingWikiMetadataSettings settings = new PCGamingWikiMetadata.PCGamingWikiMetadataSettings();
        this.gameController = new PCGWGameController(settings);
    }

    public PCGamingWikiMetadataSettings GetSettings()
    {
        return this.gameController.Settings;
    }

    public override void FetchGamePageContent(PCGWGame game)
    {
        this.gameController.Game = game;
        base.FetchGamePageContent(game);
        // game.LibraryGame = this.options.GameData;

        // JObject content = JObject.Parse(File.ReadAllText($"./data/{game.Name}.json"));

        // PCGamingWikiJSONParser jsonParser = new PCGamingWikiJSONParser(content, this.gameController);
        // PCGamingWikiHTMLParser parser = new PCGamingWikiHTMLParser(jsonParser.PageHTMLText(), this.gameController);

        // jsonParser.ParseGameDataJson();
        // parser.ApplyGameMetadata();
    }
}
