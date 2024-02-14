using Xunit;
using PCGamingWikiMetadata;
using System;
using System.Linq;
using FluentAssertions;

public class PCGWGame_Test_AC_INDIA : IDisposable
{
    private PCGWGame testGame;
    private LocalPCGWClient client;
    private TestMetadataRequestOptions options;

    public PCGWGame_Test_AC_INDIA()
    {
        this.testGame = new PCGWGame("ac_chronicles_india", -1);
        this.options = new TestMetadataRequestOptions();
        this.options.SetGameSourceBattleNet();
        this.client = new LocalPCGWClient(this.options);
        this.client.FetchGamePageContent(this.testGame);
    }

    [Fact]
    public void TestParseSeries()
    {
        var arr = this.testGame.Series.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Assassin's Creed Chronicles");
        arr.Should().Contain("Assassin's Creed");
    }

    public void Dispose()
    {

    }
}
