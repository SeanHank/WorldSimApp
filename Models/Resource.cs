using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public class Resource
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("importance")]
    public double Importance { get; set; } = 1.0;
}

public class CountryResource
{
    [JsonPropertyName("countryId")]
    public string CountryId { get; set; } = string.Empty;
    
    [JsonPropertyName("resourceId")]
    public string ResourceId { get; set; } = string.Empty;
    
    [JsonPropertyName("production")]
    public double Production { get; set; }
    
    [JsonPropertyName("consumption")]
    public double Consumption { get; set; }
    
    [JsonPropertyName("reserves")]
    public double Reserves { get; set; }
    
    [JsonPropertyName("price")]
    public double Price { get; set; } = 1.0;

    public double Surplus => Production - Consumption;
    public double SelfSufficiency => Consumption > 0 ? Math.Min(Production / Consumption * 100, 150) : 100;
}

public class TradeRoute
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [JsonPropertyName("exporterId")]
    public string ExporterId { get; set; } = string.Empty;
    
    [JsonPropertyName("importerId")]
    public string ImporterId { get; set; } = string.Empty;
    
    [JsonPropertyName("resourceId")]
    public string ResourceId { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public double Amount { get; set; }
    
    [JsonPropertyName("value")]
    public double Value { get; set; }
    
    [JsonPropertyName("turnEstablished")]
    public int TurnEstablished { get; set; }

    public bool IsActive { get; set; } = true;
    
    public double TransportCost { get; set; }
    public double TariffRate { get; set; }
    public double Distance { get; set; }
    public double BasePrice { get; set; }
    public double CurrentPrice { get; set; }
    public double PriceVolatility { get; set; }
}

public class CountryResourceData
{
    [JsonPropertyName("resources")]
    public List<Resource> Resources { get; set; } = new();

    [JsonPropertyName("countryResources")]
    public List<CountryResource> CountryResources { get; set; } = new();

    [JsonPropertyName("tradeRoutes")]
    public List<TradeRoute> TradeRoutes { get; set; } = new();
}
