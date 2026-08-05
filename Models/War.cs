using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public enum WarStatus
{
    None,
    Tensions,
    War,
    Ceasefire,
    Peace
}

public enum WarResult
{
    Ongoing,
    AttackerWins,
    DefenderWins,
    Stalemate,
    NegotiatedPeace
}

public class War
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("attackerId")]
    public string AttackerId { get; set; } = string.Empty;

    [JsonPropertyName("defenderId")]
    public string DefenderId { get; set; } = string.Empty;

    [JsonPropertyName("startTurn")]
    public int StartTurn { get; set; }

    [JsonPropertyName("currentTurn")]
    public int CurrentTurn { get; set; }

    [JsonPropertyName("status")]
    public WarStatus Status { get; set; } = WarStatus.War;

    [JsonPropertyName("result")]
    public WarResult Result { get; set; } = WarResult.Ongoing;

    [JsonPropertyName("attackerDeaths")]
    public int AttackerDeaths { get; set; }

    [JsonPropertyName("defenderDeaths")]
    public int DefenderDeaths { get; set; }

    [JsonPropertyName("attackerLosses")]
    public double AttackerLosses { get; set; }

    [JsonPropertyName("defenderLosses")]
    public double DefenderLosses { get; set; }

    [JsonPropertyName("casualtiesPerTurn")]
    public int CasualtiesPerTurn { get; set; }

    [JsonPropertyName("gdpDamage")]
    public double GdpDamage { get; set; }

    [JsonPropertyName("occupation")]
    public bool Occupation { get; set; }

    [JsonPropertyName("reparations")]
    public double Reparations { get; set; }

    public string TerrainType { get; set; } = string.Empty;
    public double DefenseBonus { get; set; } = 1.0;
    public double SupplyDifficulty { get; set; } = 1.0;
    public bool AttackerAdvances { get; set; }
    public Dictionary<string, double> AlliedForces { get; set; } = new();
}

public class Territory
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("ownerId")]
    public string OwnerId { get; set; } = string.Empty;

    [JsonPropertyName("previousOwnerId")]
    public string? PreviousOwnerId { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; } = 1.0;

    [JsonPropertyName("resourceBonus")]
    public double ResourceBonus { get; set; } = 1.0;
}
