namespace EnadeAds.Tests;

/// <summary>
/// Os testes que sobem a aplicação compartilham um único host: o bootstrap logger do Serilog
/// só pode ser congelado uma vez por processo.
/// </summary>
[CollectionDefinition(Name)]
public sealed class AppCollection : ICollectionFixture<EnadeAdsFactory>
{
    public const string Name = "Aplicação";
}
