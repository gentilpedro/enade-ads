using System.Net;

namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class HealthTests(EnadeAdsFactory factory)
{
    [Fact]
    public async Task Health_AnswersOnceTheAppIsUp()
    {
        // O deploy na VPS só dá a versão como no ar quando /health responde 200; se não, volta a anterior.
        var response = await factory.CreateClient().GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
