using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public class GameState
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("turn")]
    public int Turn { get; set; } = 1;

    [JsonPropertyName("countries")]
    public List<Country> Countries { get; set; } = new();

    [JsonPropertyName("events")]
    public List<SimulationEvent> Events { get; set; } = new();

    [JsonPropertyName("decisions")]
    public List<DecisionEvent> Decisions { get; set; } = new();

    [JsonPropertyName("wars")]
    public List<War> Wars { get; set; } = new();

    [JsonPropertyName("territories")]
    public List<Territory> Territories { get; set; } = new();

    [JsonPropertyName("organizations")]
    public List<InternationalOrganization> Organizations { get; set; } = new();

    [JsonPropertyName("resources")]
    public List<Resource> Resources { get; set; } = new();

    [JsonPropertyName("countryResources")]
    public List<CountryResource> CountryResources { get; set; } = new();

    [JsonPropertyName("tradeRoutes")]
    public List<TradeRoute> TradeRoutes { get; set; } = new();

    [JsonPropertyName("playerCountryId")]
    public string? PlayerCountryId { get; set; }

    [JsonPropertyName("settings")]
    public GameSettings Settings { get; set; } = new();

    [JsonPropertyName("gdpHistory")]
    public Dictionary<string, List<double>> GdpHistory { get; set; } = new();

    [JsonPropertyName("stabilityHistory")]
    public Dictionary<string, List<double>> StabilityHistory { get; set; } = new();

    [JsonPropertyName("worldStabilityHistory")]
    public List<double> WorldStabilityHistory { get; set; } = new();

    [JsonPropertyName("worldGdpHistory")]
    public List<double> WorldGdpHistory { get; set; } = new();

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("lastSavedAt")]
    public DateTime LastSavedAt { get; set; } = DateTime.Now;
}

public class GameStatistics
{
    public int TotalWars { get; set; }
    public int TotalTreaties { get; set; }
    public int TotalElections { get; set; }
    public int TotalDisasters { get; set; }
    public long TotalDeaths { get; set; }
    public double TotalTradeVolume { get; set; }
    public string MostPowerfulCountry { get; set; } = "";
    public string RichestCountry { get; set; } = "";
    public string MostPopulousCountry { get; set; } = "";
}
