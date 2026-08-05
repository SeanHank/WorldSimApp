using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorldSimApp.Models;
using WorldSimApp.Simulation;

namespace WorldSimApp.Models;

public class WorldSimulation
{
    private readonly Random _random = new();
    private int _currentTurn = 0;

    private CountryAnalyzer? _countryAnalyzer;
    private TradeSimulator? _tradeSimulator;
    private EventGenerator? _eventGenerator;

    private EnhancedEconomicSystem? _enhancedEconomicSystem;
    private PoliticalSystem? _enhancedPoliticalSystem;
    private MilitarySystem? _enhancedMilitarySystem;
    private DiplomaticSystem? _enhancedDiplomaticSystem;
    private SocialSystem? _enhancedSocialSystem;
    private EnhancedWarSimulator? _enhancedWarSystem;
    private AiBehavior? _enhancedAIBehavior;
    private EventSystem? _enhancedEventSystem;

    public int CurrentTurn => _currentTurn;

    public List<Country> Countries { get; private set; } = new();
    public List<SimulationEvent> Events { get; private set; } = new();
    public List<DecisionEvent> Decisions { get; private set; } = new();
    public List<War> Wars { get; private set; } = new();
    public List<InternationalOrganization> Organizations { get; private set; } = new();
    public List<Resource> Resources { get; private set; } = new();
    public List<CountryResource> CountryResources { get; private set; } = new();
    public List<TradeRoute> TradeRoutes { get; private set; } = new();
    public List<Territory> Territories { get; private set; } = new();
    
    public bool IsRunning { get; set; }
    public int Speed { get; set; } = 1000;
    public GameSettings Settings { get; set; } = new();
    public string? PlayerCountryId { get; set; }

    public Dictionary<string, List<double>> GdpHistory { get; private set; } = new();
    public Dictionary<string, List<double>> StabilityHistory { get; private set; } = new();
    public List<double> WorldStabilityHistory { get; private set; } = new();
    public List<double> WorldGdpHistory { get; private set; } = new();
    public List<GeopoliticalFactor> GeopoliticalFactors { get; private set; } = new();
    public GameStatistics Statistics { get; private set; } = new();

    private void InitializeSimulators()
    {
        _countryAnalyzer = new CountryAnalyzer(this, Settings);
        _tradeSimulator = new TradeSimulator(this, Settings);
        _eventGenerator = new EventGenerator(this, Settings);

        _enhancedEconomicSystem = new EnhancedEconomicSystem(this);
        _enhancedPoliticalSystem = new PoliticalSystem(this);
        _enhancedMilitarySystem = new MilitarySystem(this);
        _enhancedDiplomaticSystem = new DiplomaticSystem(this);
        _enhancedSocialSystem = new SocialSystem(this);
        _enhancedWarSystem = new EnhancedWarSimulator(this, Settings);
        _enhancedAIBehavior = new AiBehavior(this, Settings);
        _enhancedEventSystem = new EventSystem(this, Settings);
    }

    public void LoadCountries(string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        var data = JsonSerializer.Deserialize(json, WorldSimJsonContext.Default.CountryData);
        if (data?.Countries != null)
        {
            Countries = data.Countries;
            foreach (var c in Countries)
            {
                GdpHistory[c.Id] = new List<double> { c.Gdp };
                StabilityHistory[c.Id] = new List<double> { c.Stability };
                InitializeCountryDefaults(c);
            }
        }
    }

    private void InitializeCountryDefaults(Country c)
    {
        double gdpPerCapita = c.Population > 0 ? c.Gdp / c.Population : 0;
        
        c.LaborForce = c.Population * 0.6;
        c.PopulationUnder18 = (long)(c.Population * 0.2);
        c.Population18_35 = (long)(c.Population * 0.25);
        c.Population36_60 = (long)(c.Population * 0.35);
        c.PopulationOver60 = c.Population - c.PopulationUnder18 - c.Population18_35 - c.Population36_60;
        
        c.CurrentElectionTurn = 1;
        c.NextElectionTurn = c.ElectionCycleYears;
        
        c.ArmyPower = (int)(c.MilitaryPower * 0.4);
        c.NavyPower = (int)(c.MilitaryPower * 0.3);
        c.AirPower = (int)(c.MilitaryPower * 0.3);
        
        InitializeEconomicIndicators(c, gdpPerCapita);
        InitializeDemographics(c, gdpPerCapita);
        InitializeSocialIndicators(c, gdpPerCapita);
        
        c.PotentialGdp = c.Gdp;
        c.CapitalStock = c.Gdp * 2;
        c.GdpPerCapita = gdpPerCapita;
        c.ExchangeRate = 1.0;
        c.CurrencyStrength = 100.0;
        
        DetermineRegionalPower(c);
        
        Territories.Add(new Territory
        {
            Id = $"{c.Id}_core",
            Name = $"{c.Name} Core Territory",
            OwnerId = c.Id,
            Value = 1.0,
            ResourceBonus = 1.2
        });

        InitializeGeopoliticalFactor(c);
    }

    private void InitializeEconomicIndicators(Country c, double gdpPerCapita)
    {
        c.Exports = c.Gdp * 0.12;
        c.Imports = c.Gdp * 0.13;
        c.TradeBalance = c.Exports - c.Imports;
        c.CurrentAccount = c.TradeBalance - c.Gdp * 0.02;
        
        if (gdpPerCapita > 50000)
        {
            c.EconomicGrowth = 1.5 + RandomDouble() * 1.0;
            c.Inflation = 1.5 + RandomDouble() * 1.5;
            c.Unemployment = 3.5 + RandomDouble() * 2.5;
            c.InterestRate = 1.5 + RandomDouble() * 1.5;
            c.AgriculturePercent = 1.5 + RandomDouble() * 2;
            c.ManufacturingPercent = 10 + RandomDouble() * 8;
            c.ServicesPercent = 70 + RandomDouble() * 10;
            c.TechnologyPercent = 12 + RandomDouble() * 8;
            c.InvestmentRate = 18 + RandomDouble() * 4;
            c.SavingsRate = 22 + RandomDouble() * 5;
            c.GovernmentSpending = 15 + RandomDouble() * 5;
        }
        else if (gdpPerCapita > 25000)
        {
            c.EconomicGrowth = 2.0 + RandomDouble() * 1.5;
            c.Inflation = 2.0 + RandomDouble() * 2.0;
            c.Unemployment = 5.0 + RandomDouble() * 4;
            c.InterestRate = 2.0 + RandomDouble() * 2.0;
            c.AgriculturePercent = 4 + RandomDouble() * 4;
            c.ManufacturingPercent = 18 + RandomDouble() * 10;
            c.ServicesPercent = 55 + RandomDouble() * 10;
            c.TechnologyPercent = 8 + RandomDouble() * 6;
            c.InvestmentRate = 20 + RandomDouble() * 5;
            c.SavingsRate = 23 + RandomDouble() * 5;
            c.GovernmentSpending = 18 + RandomDouble() * 5;
        }
        else if (gdpPerCapita > 10000)
        {
            c.EconomicGrowth = 3.5 + RandomDouble() * 2.5;
            c.Inflation = 3.5 + RandomDouble() * 3.0;
            c.Unemployment = 7.0 + RandomDouble() * 5;
            c.InterestRate = 4.0 + RandomDouble() * 3.0;
            c.AgriculturePercent = 10 + RandomDouble() * 8;
            c.ManufacturingPercent = 25 + RandomDouble() * 10;
            c.ServicesPercent = 45 + RandomDouble() * 10;
            c.TechnologyPercent = 5 + RandomDouble() * 5;
            c.InvestmentRate = 25 + RandomDouble() * 8;
            c.SavingsRate = 26 + RandomDouble() * 6;
            c.GovernmentSpending = 15 + RandomDouble() * 8;
        }
        else
        {
            c.EconomicGrowth = 4.5 + RandomDouble() * 3.0;
            c.Inflation = 5.0 + RandomDouble() * 4.0;
            c.Unemployment = 10.0 + RandomDouble() * 8;
            c.InterestRate = 6.0 + RandomDouble() * 4.0;
            c.AgriculturePercent = 20 + RandomDouble() * 15;
            c.ManufacturingPercent = 20 + RandomDouble() * 10;
            c.ServicesPercent = 35 + RandomDouble() * 10;
            c.TechnologyPercent = 3 + RandomDouble() * 4;
            c.InvestmentRate = 28 + RandomDouble() * 10;
            c.SavingsRate = 28 + RandomDouble() * 8;
            c.GovernmentSpending = 12 + RandomDouble() * 10;
        }
        
        c.BaseInterestRate = c.InterestRate;
        c.RealInterestRate = c.InterestRate - c.Inflation;
        c.CapacityUtilization = 75 + RandomDouble() * 15;
        c.TotalFactorProductivity = 0.8 + RandomDouble() * 0.3;
        c.DepreciationRate = 0.04 + RandomDouble() * 0.03;
        c.LaborForceGrowth = 0.003 + RandomDouble() * 0.012;
    }

    private void InitializeDemographics(Country c, double gdpPerCapita)
    {
        if (gdpPerCapita > 50000)
        {
            c.MedianAge = 38 + RandomDouble() * 8;
            c.AgingIndex = 0.6 + RandomDouble() * 0.4;
            c.ImmigrationRate = 0.3 + RandomDouble() * 0.5;
            c.EmigrationRate = 0.1 + RandomDouble() * 0.2;
            c.NetMigration = c.ImmigrationRate - c.EmigrationRate;
            c.FertilityRate = 1.3 + RandomDouble() * 0.4;
            c.NaturalPopulationGrowth = 0.002 + RandomDouble() * 0.005;
            c.UrbanizationRate = 80 + RandomDouble() * 15;
            c.LaborForceParticipation = 62 + RandomDouble() * 10;
            c.PensionPressure = 25 + RandomDouble() * 15;
        }
        else if (gdpPerCapita > 25000)
        {
            c.MedianAge = 32 + RandomDouble() * 8;
            c.AgingIndex = 0.4 + RandomDouble() * 0.3;
            c.ImmigrationRate = 0.2 + RandomDouble() * 0.3;
            c.EmigrationRate = 0.15 + RandomDouble() * 0.2;
            c.NetMigration = c.ImmigrationRate - c.EmigrationRate;
            c.FertilityRate = 1.5 + RandomDouble() * 0.5;
            c.NaturalPopulationGrowth = 0.005 + RandomDouble() * 0.008;
            c.UrbanizationRate = 65 + RandomDouble() * 20;
            c.LaborForceParticipation = 60 + RandomDouble() * 12;
            c.PensionPressure = 15 + RandomDouble() * 10;
        }
        else if (gdpPerCapita > 10000)
        {
            c.MedianAge = 28 + RandomDouble() * 6;
            c.AgingIndex = 0.25 + RandomDouble() * 0.2;
            c.ImmigrationRate = 0.1 + RandomDouble() * 0.2;
            c.EmigrationRate = 0.2 + RandomDouble() * 0.3;
            c.NetMigration = c.ImmigrationRate - c.EmigrationRate;
            c.FertilityRate = 1.8 + RandomDouble() * 0.7;
            c.NaturalPopulationGrowth = 0.008 + RandomDouble() * 0.01;
            c.UrbanizationRate = 50 + RandomDouble() * 20;
            c.LaborForceParticipation = 58 + RandomDouble() * 12;
            c.PensionPressure = 8 + RandomDouble() * 8;
        }
        else
        {
            c.MedianAge = 22 + RandomDouble() * 6;
            c.AgingIndex = 0.15 + RandomDouble() * 0.15;
            c.ImmigrationRate = 0.05 + RandomDouble() * 0.15;
            c.EmigrationRate = 0.3 + RandomDouble() * 0.4;
            c.NetMigration = c.ImmigrationRate - c.EmigrationRate;
            c.FertilityRate = 2.5 + RandomDouble() * 1.0;
            c.NaturalPopulationGrowth = 0.012 + RandomDouble() * 0.015;
            c.UrbanizationRate = 35 + RandomDouble() * 20;
            c.LaborForceParticipation = 55 + RandomDouble() * 15;
            c.PensionPressure = 5 + RandomDouble() * 5;
        }
        
        c.YouthUnemployment = c.Unemployment * (1.2 + RandomDouble() * 0.6);
    }

    private void InitializeSocialIndicators(Country c, double gdpPerCapita)
    {
        double baseEducation = CalculateBaseEducation(c, gdpPerCapita);
        
        c.EducationLevel = Math.Clamp(baseEducation + (RandomDouble() - 0.5) * 8, 15, 90);
        
        c.LiteracyRate = Math.Clamp(60 + c.EducationLevel * 0.4 + (RandomDouble() - 0.5) * 5, 40, 99);
        
        double tertiaryBase = (c.EducationLevel - 30) * 0.6;
        c.TertiaryEnrollmentRate = Math.Clamp(tertiaryBase + (RandomDouble() - 0.5) * 8, 3, 55);
        
        double stemBase = c.TertiaryEnrollmentRate * (c.TechnologyPercent / 60);
        c.StemGraduatesRate = Math.Clamp(stemBase + (RandomDouble() - 0.5) * 3, 2, 25);
        
        double baseHealthcare = 30 + c.EducationLevel * 0.4 + gdpPerCapita / 2000;
        c.HealthcareLevel = Math.Clamp(baseHealthcare + (RandomDouble() - 0.5) * 10, 25, 95);
        
        double lifeBase = 50 + c.HealthcareLevel * 0.35 + c.EducationLevel * 0.15;
        c.LifeExpectancy = Math.Clamp(lifeBase + (RandomDouble() - 0.5) * 4, 45, 85);
        
        double infantBase = 80 - c.HealthcareLevel * 0.6 - c.EducationLevel * 0.2;
        c.InfantMortalityRate = Math.Clamp(Math.Abs(infantBase + (RandomDouble() - 0.5) * 8), 1, 60);
        
        c.HealthcareCost = Math.Clamp(gdpPerCapita / 8000 + (RandomDouble() - 0.5) * 2, 1, 15);
        
        c.Happiness = Math.Clamp(40 + c.EducationLevel * 0.2 + c.Stability * 0.3 + (RandomDouble() - 0.5) * 10, 20, 90);
        
        c.CrimeRate = Math.Clamp(15 - c.EducationLevel * 0.1 - c.Stability * 0.05 + (RandomDouble() - 0.5) * 4, 1, 30);
        
        c.CorruptionIndex = Math.Clamp(60 - c.EducationLevel * 0.3 - gdpPerCapita / 1000 + (RandomDouble() - 0.5) * 10, 5, 80);
        
        c.IncomeInequality = Math.Clamp(45 - c.EducationLevel * 0.15 + (RandomDouble() - 0.5) * 8, 20, 60);
        
        c.SocialMobility = Math.Clamp(c.EducationLevel * 0.5 + c.Stability * 0.3 + (RandomDouble() - 0.5) * 8, 10, 70);
        
        c.InfrastructureQuality = Math.Clamp(30 + c.EducationLevel * 0.3 + gdpPerCapita / 2000 + (RandomDouble() - 0.5) * 10, 20, 95);
        
        c.LawEnforcementSpending = Math.Clamp(1 + (RandomDouble() - 0.5) * 0.8, 0.5, 3);
        
        c.MiddleClassPercent = Math.Clamp(100 - c.IncomeInequality * 0.7, 20, 70);
        c.GovernmentApproval = Math.Clamp(45 + (RandomDouble() - 0.5) * 20, 20, 80);
        c.PublicOpinion = Math.Clamp(40 + (RandomDouble() - 0.5) * 20, 15, 85);
        
        c.HegemonyDesire = Math.Clamp(0.2 + (RandomDouble() - 0.5) * 0.3, 0.05, 0.8);
        c.DiplomaticCredibility = Math.Clamp(50 + c.Stability * 0.4 + (RandomDouble() - 0.5) * 15, 20, 95);
    }

    private double CalculateBaseEducation(Country c, double gdpPerCapita)
    {
        double baseValue = 30;
        
        baseValue += Math.Log10(gdpPerCapita + 5000) * 6;
        
        var regionBonus = c.Region switch
        {
            "North America" => 18,
            "Western Europe" => 18,
            "Eastern Europe" => 10,
            "Asia" => 8,
            "Middle East" => 5,
            "Latin America" => 6,
            "Africa" => -8,
            "Oceania" => 15,
            _ => 0
        };
        baseValue += regionBonus;
        
        var ideologyBonus = c.Ideology switch
        {
            "Democracy" => 6,
            "Communist" => 4,
            "Authoritarian" => -3,
            "Monarchy" => 3,
            "Theocracy" => -2,
            _ => 0
        };
        baseValue += ideologyBonus;
        
        if (c.Stability > 80) baseValue += 6;
        else if (c.Stability < 40) baseValue -= 10;
        else if (c.Stability < 60) baseValue -= 3;
        
        if (c.CorruptionIndex < 20) baseValue += 6;
        else if (c.CorruptionIndex > 60) baseValue -= 6;
        
        if (c.Population > 1e8)
        {
            baseValue -= (c.Population > 5e8 ? 8 : 4);
        }
        
        return Math.Clamp(baseValue, 20, 92);
    }

    private void DetermineRegionalPower(Country c)
    {
        double militaryShare = c.MilitaryPower / 1000.0;
        
        if (c.Gdp > 1e13 || (c.MilitaryPower > 500 && c.Population > 1e8))
            c.RegionalPower = "Superpower";
        else if (c.Gdp > 5e12 || (c.MilitaryPower > 300 && c.Population > 5e7))
            c.RegionalPower = "GreatPower";
        else if (c.Gdp > 1e12 || (c.MilitaryPower > 100 && c.Population > 2e7))
            c.RegionalPower = "RegionalPower";
        else if (c.Gdp > 1e11 || c.MilitaryPower > 50)
            c.RegionalPower = "MediumPower";
        else
            c.RegionalPower = "SmallPower";
    }

    private void InitializeGeopoliticalFactor(Country c)
    {
        var region = c.Region;
        if (string.IsNullOrEmpty(region)) return;
        
        var existingFactor = GeopoliticalFactors.FirstOrDefault(g => g.Region == region);
        if (existingFactor == null)
        {
            GeopoliticalFactors.Add(new GeopoliticalFactor
            {
                Region = region,
                DominantCountryId = c.Id,
                PowerBalance = 0.5,
                RegionalPowers = new List<string> { c.Id },
                IsContested = false
            });
        }
        else
        {
            if (c.MilitaryPower > Countries.FirstOrDefault(x => x.Id == existingFactor.DominantCountryId)?.MilitaryPower)
            {
                existingFactor.DominantCountryId = c.Id;
            }
            if (!existingFactor.RegionalPowers.Contains(c.Id))
            {
                existingFactor.RegionalPowers.Add(c.Id);
            }
            if (existingFactor.RegionalPowers.Count > 2)
            {
                existingFactor.IsContested = true;
            }
        }
    }

    private double RandomDouble()
    {
        return _random.NextDouble();
    }

    public void LoadResources(string jsonPath)
    {
        if (!File.Exists(jsonPath)) return;
        var json = File.ReadAllText(jsonPath);
        var data = JsonSerializer.Deserialize(json, WorldSimJsonContext.Default.CountryResourceData);
        if (data != null)
        {
            Resources = data.Resources;
            CountryResources = data.CountryResources;
            TradeRoutes = data.TradeRoutes;
            
            InitializeResourcePrices();
            CreateInitialTradeRoutes();
            
            System.Diagnostics.Debug.WriteLine($"WorldSimulation.LoadResources: Loaded {TradeRoutes.Count} routes");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("WorldSimulation.LoadResources: data is null");
        }
    }

    private void InitializeResourcePrices()
    {
        var resourcePrices = new Dictionary<string, double>
        {
            ["oil"] = 2000,
            ["natural_gas"] = 1200,
            ["coal"] = 800,
            ["uranium"] = 5000,
            ["rare_earth"] = 8000,
            ["iron"] = 1000,
            ["gold"] = 50000,
            ["copper"] = 3000,
            ["wheat"] = 500,
            ["corn"] = 450,
            ["rice"] = 600,
            ["silicon"] = 3000,
            ["timber"] = 400,
            ["cotton"] = 350,
            ["soybeans"] = 800
        };
        
        foreach (var cr in CountryResources)
        {
            if (cr.Price <= 1)
            {
                cr.Price = resourcePrices.GetValueOrDefault(cr.ResourceId.ToLower(), 500);
            }
        }
    }

    private void CreateInitialTradeRoutes()
    {
        if (TradeRoutes.Count > 0) return;
        
        var majorTradePairs = new[]
        {
            ("usa", "china"), ("usa", "canada"), ("usa", "mexico"), ("usa", "japan"), ("usa", "germany"),
            ("china", "japan"), ("china", "south_korea"), ("china", "australia"), ("china", "brazil"),
            ("germany", "france"), ("germany", "netherlands"), ("germany", "poland"),
            ("russia", "china"), ("russia", "germany"),
            ("saudi_arabia", "china"), ("saudi_arabia", "japan"), ("saudi_arabia", "india"),
            ("brazil", "china"), ("brazil", "usa"),
            ("australia", "china"), ("australia", "japan"),
            ("india", "china"), ("india", "usa"),
            ("canada", "usa"), ("mexico", "usa")
        };
        
        foreach (var (exporterId, importerId) in majorTradePairs)
        {
            var exporterRes = CountryResources
                .Where(r => r.CountryId == exporterId && r.Surplus > 0)
                .OrderByDescending(r => r.Surplus * r.Price)
                .Take(2)
                .ToList();
            
            foreach (var res in exporterRes)
            {
                var importerRes = CountryResources
                    .FirstOrDefault(r => r.CountryId == importerId && r.ResourceId == res.ResourceId);
                
                double demand = importerRes != null && importerRes.Surplus < 0 
                    ? -importerRes.Surplus 
                    : importerRes?.Consumption * 0.3 ?? 50;
                
                double amount = Math.Min(res.Surplus * 0.4, demand);
                amount = Math.Max(amount, 100);
                
                double distance = _random.Next(500, 8000);
                double transportCost = CalculateInitialTransportCost(distance, res.ResourceId);
                double tariffRate = CalculateInitialTariffRate(exporterId, importerId);
                
                double tradeValue = amount * res.Price * (1 - transportCost) * (1 - tariffRate / 100);
                
                TradeRoutes.Add(new TradeRoute
                {
                    ExporterId = exporterId,
                    ImporterId = importerId,
                    ResourceId = res.ResourceId,
                    Amount = amount,
                    Value = 0,
                    TurnEstablished = 0,
                    Distance = distance,
                    BasePrice = res.Price,
                    CurrentPrice = res.Price,
                    PriceVolatility = 0.1,
                    TransportCost = transportCost,
                    TariffRate = tariffRate,
                    IsActive = true
                });
            }
        }
        
        System.Diagnostics.Debug.WriteLine($"Created {TradeRoutes.Count} initial trade routes");
    }

    private double CalculateInitialTransportCost(double distance, string resourceId)
    {
        double baseCost = 0.08;
        
        var highValue = new[] { "gold", "uranium", "rare_earth" };
        if (highValue.Contains(resourceId.ToLower())) baseCost *= 0.5;
        
        var bulky = new[] { "coal", "iron", "corn", "wheat" };
        if (bulky.Contains(resourceId.ToLower())) baseCost *= 1.3;
        
        return Math.Clamp(baseCost * distance / 5000, 0.03, 0.25);
    }

    private double CalculateInitialTariffRate(string exporterId, string importerId)
    {
        var freeTradePairs = new[]
        {
            ("usa", "canada"), ("usa", "mexico"), ("germany", "france"), ("germany", "netherlands"),
            ("canada", "usa"), ("mexico", "usa")
        };
        
        if (freeTradePairs.Contains((exporterId, importerId)) || freeTradePairs.Contains((importerId, exporterId)))
            return 0;
        
        var lowTariffPairs = new[]
        {
            ("usa", "japan"), ("usa", "uk"), ("usa", "germany"),
            ("china", "australia"), ("china", "south_korea"),
            ("germany", "poland")
        };
        
        if (lowTariffPairs.Contains((exporterId, importerId)) || lowTariffPairs.Contains((importerId, exporterId)))
            return 2;
        
        return 5 + _random.NextDouble() * 5;
    }

    public void LoadOrganizations(string jsonPath)
    {
        if (!File.Exists(jsonPath)) return;
        var json = File.ReadAllText(jsonPath);
        var data = JsonSerializer.Deserialize(json, WorldSimJsonContext.Default.OrganizationData);
        if (data != null)
        {
            Organizations = data.Organizations;
        }
    }

    public void Initialize()
    {
        _currentTurn = 1;
        Events.Clear();
        Decisions.Clear();
        Wars.Clear();
        
        WorldStabilityHistory.Clear();
        WorldGdpHistory.Clear();
        
        foreach (var c in Countries)
        {
            c.Turn = 1;
            GdpHistory[c.Id] = new List<double> { c.Gdp };
            StabilityHistory[c.Id] = new List<double> { c.Stability };
            InitializeCountryDefaults(c);
        }
        
        InitializeSimulators();
        RecordHistory();
    }

    public void NextTurn()
    {
        InitializeSimulators();
        
        _currentTurn++;
        
        _enhancedEconomicSystem?.SimulateEconomicChanges();
        _enhancedPoliticalSystem?.SimulatePoliticalSystem();
        _enhancedMilitarySystem?.SimulateMilitaryChanges();
        _enhancedDiplomaticSystem?.SimulateDiplomaticRelations();
        _tradeSimulator?.SimulateTrade();
        _enhancedWarSystem?.SimulateDiplomaticRelations();
        _enhancedWarSystem?.SimulateWars();
        _enhancedSocialSystem?.SimulateSocialChanges();
        _enhancedAIBehavior?.SimulateAIBehavior();
        _eventGenerator?.SimulateAiDecisions();
        _eventGenerator?.GenerateRandomEvents();
        _enhancedEventSystem?.GenerateEnhancedEvents();
        
        _countryAnalyzer?.SimulateMilitaryChanges();
        _countryAnalyzer?.SimulatePoliticalChanges();
        _countryAnalyzer?.UpdateCountryStats();
        
        ProcessDecisions();
        
        RecordHistory();
    }

    private void RecordHistory()
    {
        foreach (var c in Countries)
        {
            if (!GdpHistory.ContainsKey(c.Id)) GdpHistory[c.Id] = new();
            if (!StabilityHistory.ContainsKey(c.Id)) StabilityHistory[c.Id] = new();
            
            GdpHistory[c.Id].Add(c.Gdp);
            StabilityHistory[c.Id].Add(c.Stability);
            
            if (GdpHistory[c.Id].Count > 100) GdpHistory[c.Id].RemoveAt(0);
            if (StabilityHistory[c.Id].Count > 100) StabilityHistory[c.Id].RemoveAt(0);
        }
        
        WorldStabilityHistory.Add(Countries.Average(c => c.Stability));
        WorldGdpHistory.Add(Countries.Sum(c => c.Gdp));
        
        if (WorldStabilityHistory.Count > 100) WorldStabilityHistory.RemoveAt(0);
        if (WorldGdpHistory.Count > 100) WorldGdpHistory.RemoveAt(0);

        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        Statistics.TotalWars = Wars.Count;
        Statistics.TotalTreaties = Countries.Sum(c => c.Treaties.Count);
        Statistics.TotalElections = Countries.Sum(c => c.ElectionHistory.Count);
        Statistics.TotalDisasters = Events.Count(e => e.Type == "Disaster");
        Statistics.TotalDeaths = Wars.Sum(w => w.AttackerDeaths + w.DefenderDeaths);
        Statistics.TotalTradeVolume = TradeRoutes.Sum(t => t.Value);
        
        var mostPowerful = Countries.OrderByDescending(c => c.MilitaryPower).FirstOrDefault();
        Statistics.MostPowerfulCountry = mostPowerful?.Name ?? "";
        
        var richest = Countries.OrderByDescending(c => c.Gdp).FirstOrDefault();
        Statistics.RichestCountry = richest?.Name ?? "";
        
        var mostPopulous = Countries.OrderByDescending(c => c.Population).FirstOrDefault();
        Statistics.MostPopulousCountry = mostPopulous?.Name ?? "";
    }

    public void ProcessDecisions()
    {
        var playerDecisions = Decisions.Where(d => d.IsPlayerDecision && !d.IsResolved).ToList();
        
        foreach (var decision in playerDecisions)
        {
            if (!string.IsNullOrEmpty(decision.SelectedOptionId))
            {
                var option = decision.Options.FirstOrDefault(o => o.Id == decision.SelectedOptionId);
                if (option != null)
                {
                    var country = Countries.FirstOrDefault(c => c.Id == decision.CountryId);
                    if (country != null)
                    {
                        country.Gdp *= (1 + option.EffectGdp / 100);
                        country.Stability = Math.Clamp(country.Stability + option.EffectStability, 0, 100);
                        country.MilitaryPower = (int)(country.MilitaryPower * (1 + option.EffectMilitary / 100));
                        country.Happiness = Math.Clamp(country.Happiness + option.EffectHappiness, 0, 100);
                    }
                }
                decision.IsResolved = true;
            }
        }
    }

    public Country? GetCountry(string id)
    {
        return Countries.FirstOrDefault(c => c.Id == id);
    }

    public string GetWorldStatus()
    {
        if (Countries.Count == 0)
            return "No countries loaded";
            
        double avgStability = Countries.Average(c => c.Stability);
        long totalPop = Countries.Sum(c => c.Population);
        int activeWars = Wars.Count(w => w.Status == WarStatus.War);

        double worldGdpGrowth = CalculateWorldGdpGrowth();
        
        string status = avgStability > 70 ? "Peaceful" : avgStability > 40 ? "Tense" : "Critical";
        if (activeWars > 0) status = $"War ({activeWars} active)";
        
        return $"Turn {_currentTurn} | Status: {status} | Stability: {avgStability:F1}% | Growth: {worldGdpGrowth:F1}% | Pop: {totalPop:N0}";
    }

    public double CalculateWorldGdpGrowth()
    {
        if (WorldGdpHistory.Count < 2)
            return 0;
        
        double currentGdp = WorldGdpHistory[^1];
        double previousGdp = WorldGdpHistory[^2];
        
        if (previousGdp <= 0)
            return 0;
        
        return (currentGdp - previousGdp) / previousGdp * 100;
    }

    private void CleanupInvalidValues()
    {
        foreach (var country in Countries)
        {
            CountryDoubleProperties.CleanupInvalidValues(country);
        }
    }

    public GameState SaveGame()
    {
        CleanupInvalidValues();
        
        return new GameState
        {
            Turn = _currentTurn,
            Countries = Countries,
            Events = Events,
            Decisions = Decisions,
            Wars = Wars,
            Organizations = Organizations,
            Resources = Resources,
            CountryResources = CountryResources,
            TradeRoutes = TradeRoutes,
            PlayerCountryId = PlayerCountryId,
            Settings = Settings,
            GdpHistory = GdpHistory,
            StabilityHistory = StabilityHistory,
            WorldStabilityHistory = WorldStabilityHistory,
            WorldGdpHistory = WorldGdpHistory,
            LastSavedAt = DateTime.Now
        };
    }

    public void LoadGame(GameState state)
    {
        _currentTurn = state.Turn;
        Countries = state.Countries;
        Events = state.Events;
        Decisions = state.Decisions;
        Wars = state.Wars;
        Organizations = state.Organizations;
        Resources = state.Resources;
        CountryResources = state.CountryResources;
        TradeRoutes = state.TradeRoutes;
        PlayerCountryId = state.PlayerCountryId;
        Settings = state.Settings;
        GdpHistory = state.GdpHistory;
        StabilityHistory = state.StabilityHistory;
        WorldStabilityHistory = state.WorldStabilityHistory;
        WorldGdpHistory = state.WorldGdpHistory;
        
        InitializeSimulators();
    }

    public List<double> GetCountryGdpHistory(string countryId)
    {
        return GdpHistory.TryGetValue(countryId, out var history) ? history : new List<double>();
    }

    public List<double> GetCountryStabilityHistory(string countryId)
    {
        return StabilityHistory.TryGetValue(countryId, out var history) ? history : new List<double>();
    }

    public List<TradeRoute> GetCountryTradeRoutes(string countryId)
    {
        return TradeRoutes.Where(t => t.ExporterId == countryId || t.ImporterId == countryId).ToList();
    }

    public List<War> GetActiveWars()
    {
        return Wars.Where(w => w.Status == WarStatus.War).ToList();
    }

    public List<DecisionEvent> GetPendingDecisions()
    {
        return Decisions.Where(d => d.IsPlayerDecision && !d.IsResolved).ToList();
    }

    public List<CountryResource> GetCountryResources(string countryId)
    {
        return CountryResources.Where(r => r.CountryId == countryId).ToList();
    }

    public List<Territory> GetCountryTerritories(string countryId)
    {
        return Territories.Where(t => t.OwnerId == countryId).ToList();
    }

    public List<GeopoliticalFactor> GetRegionalFactors(string region)
    {
        return GeopoliticalFactors.Where(g => g.Region == region).ToList();
    }

    public GeopoliticalFactor? GetCountryRegionFactor(string countryId)
    {
        var country = Countries.FirstOrDefault(c => c.Id == countryId);
        if (country == null) return null;
        return GeopoliticalFactors.FirstOrDefault(g => g.Region == country.Region);
    }

    public List<InternationalOrganization> GetCountryOrganizations(string countryId)
    {
        return Organizations.Where(o => o.MemberIds.Contains(countryId)).ToList();
    }
}

public class CountryData
{
    [JsonPropertyName("countries")]
    public List<Country> Countries { get; set; } = new();
}
