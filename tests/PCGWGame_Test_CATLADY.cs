using Xunit;
using PCGamingWikiMetadata;
using System;
using System.Linq;
using FluentAssertions;

public class PCGWGame_Test_CATLADY : IDisposable
{
    private PCGWGame testGame;
    private LocalPCGWClient client;
    private TestMetadataRequestOptions options;


    public PCGWGame_Test_CATLADY()
    {
        this.testGame = new PCGWGame("cat_lady", -1);
        this.options = new TestMetadataRequestOptions();
        this.options.SetGameSourceSteam();
        this.client = new LocalPCGWClient(this.options);
        // this.client.GetSettings().ImportTagNoCloudSaves = false;
        // this.client.GetSettings().ImportFeatureFramerate60 = true;
        // this.client.GetSettings().ImportFeatureFramerate120 = true;
        // this.client.GetSettings().ImportFeatureVR = true;
        this.client.FetchGamePageContent(this.testGame);
    }

    [Fact]
    public void TestTouchscreenSupport()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().Contain("Touchscreen optimised");
    }

    public void Dispose()
    {

    }
}
