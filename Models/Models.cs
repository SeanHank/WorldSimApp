using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public enum PartyIdeology
{
    FarLeft,
    Left,
    CenterLeft,
    Center,
    CenterRight,
    Right,
    FarRight
}

public class Party
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public PartyIdeology Ideology { get; set; } = PartyIdeology.Center;
    public string EconomicPolicy { get; set; } = "Market";
    public string SocialPolicy { get; set; } = "Moderate";
    public string ForeignPolicy { get; set; } = "Neutral";
    public double BaseSupport { get; set; } = 30.0;
    public double Populism { get; set; } = 0.3;
    public Dictionary<string, double> PolicyPositions { get; set; } = new();
}

public class Election
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public string CountryId { get; set; } = string.Empty;
    public int Turn { get; set; }
    public bool IsHeld { get; set; }
    public string WinnerId { get; set; } = string.Empty;
    public string WinnerName { get; set; } = string.Empty;
    public double VoterTurnout { get; set; } = 70.0;
    public Dictionary<string, double> VoteResults { get; set; } = new();
    public string KeyIssue { get; set; } = string.Empty;
    public string WinnerAgenda { get; set; } = string.Empty;
}

public class Treaty
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> Signatories { get; set; } = new();
    public int TurnSigned { get; set; }
    public int Duration { get; set; } = -1;
    public bool IsActive { get; set; } = true;
    public Dictionary<string, double> Effects { get; set; } = new();
    public string Obligation { get; set; } = string.Empty;
    public string Benefit { get; set; } = string.Empty;
}

public class TradeAgreement
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public string PartnerId { get; set; } = string.Empty;
    public double TariffRate { get; set; } = 5.0;
    public int TurnEstablished { get; set; }
    public double TradeVolume { get; set; }
    public string ResourceFocus { get; set; } = "General";
    public bool IsActive { get; set; } = true;
}

public enum SanctionType
{
    TradeEmbargo,
    ArmsEmbargo,
    FinancialSanctions,
    TravelBan,
    DiplomaticSanctions
}

public class Sanction
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public string ImposingCountryId { get; set; } = string.Empty;
    public string TargetCountryId { get; set; } = string.Empty;
    public SanctionType Type { get; set; }
    public int TurnImplemented { get; set; }
    public int Duration { get; set; } = 10;
    public int RemainingTurns { get; set; }
    public string Reason { get; set; } = string.Empty;
    public double EconomicImpact { get; set; }
    public bool IsActive { get; set; } = true;
}

public enum EconomicCyclePhase
{
    Recession,
    Recovery,
    Expansion,
    Peak
}

public class EconomicCycle
{
    public EconomicCyclePhase Phase { get; set; } = EconomicCyclePhase.Recovery;
    public int DurationInPhase { get; set; }
    public double PhaseMultiplier { get; set; } = 1.0;
    public double CrisisProbability { get; set; } = 0.02;
    public bool IsInCrisis { get; set; }
    public string CrisisType { get; set; } = string.Empty;
    public int CrisisDuration { get; set; }
}

public class ChainEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public string TriggerEventId { get; set; } = string.Empty;
    public string FollowUpEventId { get; set; } = string.Empty;
    public double TriggerProbability { get; set; } = 0.5;
    public int DelayTurns { get; set; } = 1;
    public bool HasTriggered { get; set; }
    public string Condition { get; set; } = string.Empty;
}

public class Technology
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
    public double GdpBonus { get; set; }
    public double MilitaryBonus { get; set; }
    public double HappinessBonus { get; set; }
    public int ResearchCost { get; set; }
    public int TurnDiscovered { get; set; }
    public List<string> RequiredTechnologies { get; set; } = new();
    public bool IsDiscovered { get; set; }
}

public class ClimateEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
    public int TurnStart { get; set; }
    public int Duration { get; set; }
    public double GlobalTemperatureChange { get; set; }
    public Dictionary<string, double> RegionalEffects { get; set; } = new();
    public bool IsActive { get; set; }
}

public class CountryMemory
{
    public string CountryId { get; set; } = string.Empty;
    public Dictionary<string, int> PastConflicts { get; set; } = new();
    public Dictionary<string, int> PastAlliances { get; set; } = new();
    public Dictionary<string, double> Grudges { get; set; } = new();
    public Dictionary<string, double> Favors { get; set; } = new();
    public int LastWarTurn { get; set; }
    public int LastAllianceTurn { get; set; }
}

public class GeopoliticalFactor
{
    public string Region { get; set; } = string.Empty;
    public string DominantCountryId { get; set; } = string.Empty;
    public double PowerBalance { get; set; } = 0.5;
    public List<string> RegionalPowers { get; set; } = new();
    public bool IsContested { get; set; }
    public string ContestedResource { get; set; } = string.Empty;
}
