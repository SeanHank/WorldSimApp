using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public enum DecisionType
{
    Economic,
    Military,
    Diplomatic,
    Domestic,
    Emergency,
    Trade
}

public class DecisionOption
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("effectGdp")]
    public double EffectGdp { get; set; }

    [JsonPropertyName("effectStability")]
    public double EffectStability { get; set; }

    [JsonPropertyName("effectMilitary")]
    public double EffectMilitary { get; set; }

    [JsonPropertyName("effectHappiness")]
    public double EffectHappiness { get; set; }

    [JsonPropertyName("cost")]
    public double Cost { get; set; }

    [JsonPropertyName("requiredIdeology")]
    public string? RequiredIdeology { get; set; }
}

public class DecisionEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("turn")]
    public int Turn { get; set; }

    [JsonPropertyName("countryId")]
    public string CountryId { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public DecisionType Type { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<DecisionOption> Options { get; set; } = new();

    [JsonPropertyName("isPlayerDecision")]
    public bool IsPlayerDecision { get; set; }

    [JsonPropertyName("isResolved")]
    public bool IsResolved { get; set; }

    [JsonPropertyName("selectedOptionId")]
    public string? SelectedOptionId { get; set; }
}

public class GameSettings
{
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "Normal";

    [JsonPropertyName("aiAggressiveness")]
    public double AiAggressiveness { get; set; } = 1.0;

    [JsonPropertyName("disasterFrequency")]
    public double DisasterFrequency { get; set; } = 1.0;

    [JsonPropertyName("eventFrequency")]
    public double EventFrequency { get; set; } = 1.0;

    [JsonPropertyName("warProbability")]
    public double WarProbability { get; set; } = 1.0;

    [JsonPropertyName("tradeEnabled")]
    public bool TradeEnabled { get; set; } = true;

    [JsonPropertyName("realTimeMode")]
    public bool RealTimeMode { get; set; } = false;
    
    [JsonPropertyName("randomnessMultiplier")]
    public double RandomnessMultiplier { get; set; } = 1.0;
    
    [JsonPropertyName("majorEventFrequency")]
    public double MajorEventFrequency { get; set; } = 0.03;
    
    [JsonPropertyName("businessCycleIntensity")]
    public double BusinessCycleIntensity { get; set; } = 1.0;
    
    [JsonPropertyName("contagionFactor")]
    public double ContagionFactor { get; set; } = 0.3;

    public static GameSettings Easy() => new()
    {
        Difficulty = "Easy",
        AiAggressiveness = 0.5,
        DisasterFrequency = 0.5,
        EventFrequency = 1.0,
        WarProbability = 0.5,
        RandomnessMultiplier = 0.7,
        MajorEventFrequency = 0.02,
        BusinessCycleIntensity = 0.8,
        ContagionFactor = 0.2
    };

    public static GameSettings Normal() => new()
    {
        Difficulty = "Normal",
        AiAggressiveness = 1.0,
        DisasterFrequency = 1.0,
        EventFrequency = 1.0,
        WarProbability = 1.0,
        RandomnessMultiplier = 1.0,
        MajorEventFrequency = 0.03,
        BusinessCycleIntensity = 1.0,
        ContagionFactor = 0.3
    };

    public static GameSettings Hard() => new()
    {
        Difficulty = "Hard",
        AiAggressiveness = 1.5,
        DisasterFrequency = 1.5,
        EventFrequency = 1.5,
        WarProbability = 1.5,
        RandomnessMultiplier = 1.5,
        MajorEventFrequency = 0.05,
        BusinessCycleIntensity = 1.5,
        ContagionFactor = 0.5
    };
}
