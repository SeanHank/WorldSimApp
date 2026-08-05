using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public class Country
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("population")]
    public long Population { get; set; }

    [JsonPropertyName("gdp")]
    public double Gdp { get; set; }

    [JsonPropertyName("military")]
    public int MilitaryPower { get; set; }

    [JsonPropertyName("stability")]
    public double Stability { get; set; }

    [JsonPropertyName("ideology")]
    public string Ideology { get; set; } = "Neutral";

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("allies")]
    public List<string> Allies { get; set; } = new();

    [JsonPropertyName("enemies")]
    public List<string> Enemies { get; set; } = new();

    public double EconomicGrowth { get; set; } = 2.0;
    public double Happiness { get; set; } = 70.0;
    public double Inflation { get; set; } = 2.5;
    public double Unemployment { get; set; } = 5.0;
    public int Turn { get; set; } = 1;

    public double InterestRate { get; set; } = 2.0;
    public double BaseInterestRate { get; set; } = 2.0;
    public double TradeBalance { get; set; } = 0;
    public double Exports { get; set; } = 0;
    public double Imports { get; set; } = 0;
    public double ExchangeRate { get; set; } = 1.0;
    public double CurrencyStrength { get; set; } = 1.0;

    public double AgriculturePercent { get; set; } = 10.0;
    public double ManufacturingPercent { get; set; } = 25.0;
    public double ServicesPercent { get; set; } = 50.0;
    public double TechnologyPercent { get; set; } = 15.0;

    public double YouthUnemployment { get; set; } = 15.0;
    public double LaborForce { get; set; } = 0;
    public double LaborForceParticipation { get; set; } = 60.0;

    public int CurrentElectionTurn { get; set; } = 1;
    public int ElectionCycleYears { get; set; } = 4;
    public double GovernmentApproval { get; set; } = 60.0;
    public string RulingParty { get; set; } = "Moderate";
    public string PoliticalSpectrum { get; set; } = "Center";
    public List<string> PolicyAgenda { get; set; } = new();
    public int NextElectionTurn { get; set; } = 5;

    public int ArmyPower { get; set; } = 100;
    public int NavyPower { get; set; } = 50;
    public int AirPower { get; set; } = 50;
    public double MilitarySpending { get; set; } = 2.0;
    public double MilitarySpendingPercent { get; set; } = 2.0;
    public double DefenseIndustryOutput { get; set; } = 5.0;
    public double WarFatigue { get; set; } = 0;
    public int WarsFought { get; set; } = 0;
    public int WarsWon { get; set; } = 0;

    public Dictionary<string, int> DiplomaticRelations { get; set; } = new();
    public List<Sanction> ActiveSanctions { get; set; } = new();
    public List<Sanction> SanctionedBy { get; set; } = new();
    public List<Treaty> Treaties { get; set; } = new();
    public List<TradeAgreement> TradeAgreements { get; set; } = new();

    public long PopulationUnder18 { get; set; }
    public long Population18_35 { get; set; }
    public long Population36_60 { get; set; }
    public long PopulationOver60 { get; set; }
    public double MedianAge { get; set; } = 35.0;
    public double AgingIndex { get; set; } = 0.5;
    public double ImmigrationRate { get; set; } = 0.5;
    public double EmigrationRate { get; set; } = 0.3;
    public double NetMigration { get; set; } = 0.2;
    public double EducationLevel { get; set; } = 50.0;
    public double LiteracyRate { get; set; } = 95.0;
    public double CrimeRate { get; set; } = 5.0;
    public string DominantReligion { get; set; } = "None";
    public string CultureGroup { get; set; } = "Western";

    public Dictionary<string, double> HistoricalGrievances { get; set; } = new();
    public Dictionary<string, double> ResourceDependency { get; set; } = new();
    public Dictionary<string, string> StrategicResources { get; set; } = new();
    public string RegionalPower { get; set; } = "None";
    public double HegemonyDesire { get; set; } = 0.3;
    public Dictionary<string, int> MemoryOfConflicts { get; set; } = new();
    [JsonIgnore]
    public List<Election> ElectionHistory { get; set; } = new();
    public int LastWarTurn { get; set; }
    public int LastAllianceTurn { get; set; }

    public double PotentialGdp { get; set; }
    public double RealInterestRate { get; set; }
    public double NAIRU { get; set; } = 5.0;
    public double CurrentAccount { get; set; }

    public double InvestmentRate { get; set; } = 20.0;
    public double SavingsRate { get; set; } = 25.0;
    public double GovernmentSpending { get; set; } = 18.0;
    public double CapacityUtilization { get; set; } = 80.0;
    public double CapitalStock { get; set; }
    public double DepreciationRate { get; set; } = 0.05;
    public double TotalFactorProductivity { get; set; } = 1.0;
    public double GdpPerCapita { get; set; }
    public double LaborForceGrowth { get; set; } = 0.01;

    public string PreviousRulingParty { get; set; } = string.Empty;
    public double PublicOpinion { get; set; } = 50.0;
    public double ScandalLevel { get; set; } = 0.0;
    public List<double> ApprovalTrend { get; set; } = new();
    public Dictionary<string, double> IssueSalience { get; set; } = new()
    {
        ["Economy"] = 30,
        ["Security"] = 20,
        ["Environment"] = 15
    };
    public Dictionary<string, PendingPolicy> NewPoliciesToImplement { get; set; } = new();

    public double DiplomaticCredibility { get; set; } = 70.0;
    public Dictionary<string, List<DiplomaticAction>> DiplomaticHistory { get; set; } = new();
    public Dictionary<string, int> PastTreatiesFulfilled { get; set; } = new();
    public Dictionary<string, int> PastTreatiesBroken { get; set; } = new();

    public double FertilityRate { get; set; } = 2.1;
    public double NaturalPopulationGrowth { get; set; } = 0.01;
    public double UrbanizationRate { get; set; } = 60.0;
    public double PensionPressure { get; set; } = 20.0;
    public double LaborProductivity { get; set; } = 1.0;
    public double GdpPerCapitaGrowth { get; set; } = 0.0;
    public double TertiaryEnrollmentRate { get; set; } = 20.0;
    public double StemGraduatesRate { get; set; } = 15.0;
    public double HealthcareLevel { get; set; } = 60.0;
    public double LifeExpectancy { get; set; } = 75.0;
    public double InfantMortalityRate { get; set; } = 10.0;
    public double HealthcareCost { get; set; } = 5.0;
    public long PrisonPopulation { get; set; }
    public double LawEnforcementSpending { get; set; } = 2.0;
    public double InfrastructureQuality { get; set; } = 60.0;
    public double CorruptionIndex { get; set; } = 40.0;
    public double SocialMobility { get; set; } = 30.0;
    public double IncomeInequality { get; set; } = 40.0;
    public double MiddleClassPercent { get; set; } = 50.0;

    public double MilitaryReadiness { get; set; } = 70.0;

    public CountryMemory Memory { get; set; } = new();
}
