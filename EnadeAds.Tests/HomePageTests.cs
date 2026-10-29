namespace EnadeAds.Tests;

[Collection(AppCollection.Name)]
public class HomePageTests(EnadeAdsFactory factory)
{
    [Fact]
    public async Task Home_ReturnsSuccess()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
    }
}
