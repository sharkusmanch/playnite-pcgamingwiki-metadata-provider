using Xunit;
using PCGamingWikiMetadata;
using System;
using System.Linq;
using FluentAssertions;

public class PCGWGame_Test_LMS : IDisposable
{
    private PCGWGame testGame;
    private PCGWClient client;

    public PCGWGame_Test_LMS()
    {
        this.testGame = new PCGWGame("LMS", -1);
        this.client = new LocalPCGWClient();
        this.client.FetchGamePageContent(this.testGame);
    }

    [Fact]
    public void TestParseDevelopers()
    {
        var arr = this.testGame.Developers.Select(i => i.ToString()).ToArray();
        arr.Should().BeEquivalentTo("Skyhook Games");
    }

    [Fact]
    public void TestParsePublishers()
    {
        var arr = this.testGame.Publishers.Select(i => i.ToString()).ToArray();
        arr.Should().BeEquivalentTo("Curve Digital");
    }

    [Fact]
    public void TestParseModes()
    {
        var arr = this.testGame.Features.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Singleplayer");
    }

    public void Dispose()
    {

    }
}
