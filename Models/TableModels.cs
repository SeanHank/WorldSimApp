using System.Collections.ObjectModel;

namespace WorldSimApp.Models;

public class CountryRankItem
{
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? SubValue { get; set; }
}

public class GlobalStatisticItem
{
    public string Category { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class WorldOverviewItem
{
    public string Metric { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CompareItem
{
    public string Category { get; set; } = string.Empty;
    public string Country1Label { get; set; } = string.Empty;
    public string Country1Value { get; set; } = string.Empty;
    public string Country2Label { get; set; } = string.Empty;
    public string Country2Value { get; set; } = string.Empty;
}

public class TradeRouteItem
{
    public string Exporter { get; set; } = string.Empty;
    public string Importer { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
}

public class EconomyItem
{
    public int Rank { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Gdp { get; set; } = string.Empty;
    public string Growth { get; set; } = string.Empty;
    public string Exports { get; set; } = string.Empty;
    public string TechPercent { get; set; } = string.Empty;
    public string CurrencyStrength { get; set; } = string.Empty;
    public string ManufacturingPercent { get; set; } = string.Empty;
}

public class ConflictItem
{
    public string Name { get; set; } = string.Empty;
    public string Attacker { get; set; } = string.Empty;
    public string Defender { get; set; } = string.Empty;
    public string StartTurn { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Casualties { get; set; } = string.Empty;
}

public class CountryDetailItem
{
    public string Category { get; set; } = string.Empty;
    public string Property { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
