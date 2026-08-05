using System;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public class PlayerCountryData
{
    [JsonPropertyName("country")]
    public Country Country { get; set; } = new();

    [JsonPropertyName("currentTurn")]
    public int CurrentTurn { get; set; }

    [JsonPropertyName("savedAt")]
    public DateTime SavedAt { get; set; }
}
