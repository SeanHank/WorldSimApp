using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;

namespace WorldSimApp.Simulation;

public class AiBehavior
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;
    private readonly GameSettings _settings;

    private Dictionary<string, StrategicGoal> _strategicGoals = new();
    private Dictionary<string, AllianceStrategy> _allianceStrategies = new();

    public AiBehavior(WorldSimulation simulation, GameSettings settings)
    {
        _simulation = simulation;
        _settings = settings;
        _random = new Random();
    }

    public void SimulateAIBehavior()
    {
        foreach (var country in _simulation.Countries.Where(c => c.Id != _simulation.PlayerCountryId))
        {
            InitializeOrUpdateStrategicGoals(country);
            EvaluateGeopoliticalGoals(country);
            ExecuteEconomicStrategy(country);
            HandleResourceCompetition(country);
            EvaluateAlliances(country);
            ProcessHistoricalMemory(country);
            ExecuteMilitaryStrategy(country);
            PursueRegionalInfluence(country);
        }
    }

    private void InitializeOrUpdateStrategicGoals(Country country)
    {
        if (!_strategicGoals.ContainsKey(country.Id))
        {
            _strategicGoals[country.Id] = new StrategicGoal
            {
                CountryId = country.Id,
                PrimaryObjective = DeterminePrimaryObjective(country),
                SecondaryObjectives = DetermineSecondaryObjectives(country),
                TimeHorizon = _random.Next(5, 15),
                ProgressMetrics = new Dictionary<string, double>()
            };
        }
        
        var goal = _strategicGoals[country.Id];
        goal.Progress = Math.Min(100, goal.Progress + _random.NextDouble() * 2);
        
        if (goal.Progress > 80 || _simulation.CurrentTurn % goal.TimeHorizon == 0)
        {
            goal.PrimaryObjective = DeterminePrimaryObjective(country);
            goal.SecondaryObjectives = DetermineSecondaryObjectives(country);
            goal.Progress = 0;
        }
    }

    private StrategicObjective DeterminePrimaryObjective(Country country)
    {
        if (country.MilitaryPower > 500 && country.HegemonyDesire > 0.6)
            return StrategicObjective.RegionalHegemony;
        
        if (country.EconomicGrowth < 1)
            return StrategicObjective.EconomicRecovery;
        
        if (country.Stability < 40)
            return StrategicObjective.Stabilization;
        
        if (country.RegionalPower == "None" && country.MilitaryPower > 200)
            return StrategicObjective.RegionalInfluence;
        
        return StrategicObjective.EconomicGrowth;
    }

    private List<StrategicObjective> DetermineSecondaryObjectives(Country country)
    {
        var objectives = new List<StrategicObjective>();
        
        if (country.TechnologyPercent < 20)
            objectives.Add(StrategicObjective.TechnologicalAdvancement);
        
        if (country.Allies.Count < 2)
            objectives.Add(StrategicObjective.AllianceBuilding);
        
        if (country.TradeBalance < 0)
            objectives.Add(StrategicObjective.TradeSurplus);
        
        if (country.MilitaryPower < 300)
            objectives.Add(StrategicObjective.MilitaryModernization);
        
        if (country.EducationLevel < 60)
            objectives.Add(StrategicObjective.EducationReform);
        
        return objectives.Take(3).ToList();
    }

    private void EvaluateGeopoliticalGoals(Country country)
    {
        var goal = _strategicGoals[country.Id];
        
        var regionalPowers = _simulation.Countries
            .Where(c => c.Region == country.Region && c.Id != country.Id && c.MilitaryPower > country.MilitaryPower * 0.7)
            .ToList();

        if (regionalPowers.Count > 0)
        {
            country.RegionalPower = "Challenger";
            
            if (goal.PrimaryObjective == StrategicObjective.RegionalHegemony)
            {
                country.HegemonyDesire = Math.Min(1.0, country.HegemonyDesire + 0.02);
            }
        }
        else if (country.MilitaryPower > 500)
        {
            country.RegionalPower = "Regional Power";
            country.HegemonyDesire = 0.5;
        }

        if (country.HegemonyDesire > 0.7 && _random.NextDouble() < 0.05)
        {
            var target = regionalPowers.FirstOrDefault();
            if (target != null && !country.Enemies.Contains(target.Id))
            {
                if (country.DiplomaticRelations[target.Id] < 20)
                {
                    country.Enemies.Add(target.Id);
                    target.Enemies.Add(country.Id);
                    country.MemoryOfConflicts[target.Id] = 20;
                    
                    _simulation.Events.Add(new SimulationEvent
                    {
                        Turn = _simulation.CurrentTurn,
                        CountryId = country.Id,
                        CountryName = country.Name,
                        Type = "Military",
                        Title = "Geopolitical Rivalry",
                        Description = $"{country.Name} views {target.Name} as a strategic rival in the region.",
                        ImpactStability = -3
                    });
                }
            }
        }
    }

    private void ExecuteEconomicStrategy(Country country)
    {
        var goal = _strategicGoals[country.Id];
        
        switch (goal.PrimaryObjective)
        {
            case StrategicObjective.EconomicGrowth:
                if (country.EconomicGrowth < 2)
                {
                    country.MilitarySpending = Math.Max(1, country.MilitarySpending - 0.5);
                    country.EducationLevel += 0.5;
                }
                break;
                
            case StrategicObjective.EconomicRecovery:
                if (country.EconomicGrowth < 0)
                {
                    if (!country.PolicyAgenda.Contains("Stimulus"))
                    {
                        country.EconomicGrowth += 1.5;
                        country.Inflation += 1;
                    }
                }
                break;
                
            case StrategicObjective.TradeSurplus:
                var exportPartners = _simulation.Countries
                    .Where(c => c.Id != country.Id && !country.Enemies.Contains(c.Id))
                    .OrderByDescending(c => country.DiplomaticRelations.GetValueOrDefault(c.Id, 0))
                    .Take(3);
                
                foreach (var partner in exportPartners)
                {
                    if (!country.TradeAgreements.Any(t => t.PartnerId == partner.Id))
                    {
                        var newAgreement = new TradeAgreement
                        {
                            PartnerId = partner.Id,
                            TariffRate = _random.NextDouble() * 5,
                            TurnEstablished = _simulation.CurrentTurn,
                            TradeVolume = country.Gdp * 0.01,
                            IsActive = true
                        };
                        country.TradeAgreements.Add(newAgreement);
                        break;
                    }
                }
                break;
        }
    }

    private void ExecuteMilitaryStrategy(Country country)
    {
        var goal = _strategicGoals[country.Id];
        
        switch (goal.PrimaryObjective)
        {
            case StrategicObjective.RegionalHegemony:
            case StrategicObjective.RegionalInfluence:
                if (country.WarFatigue < 40)
                {
                    country.MilitarySpending = Math.Min(10, country.MilitarySpending + 0.3);
                    
                    if (country.TechnologyPercent < 40)
                    {
                        country.TechnologyPercent = Math.Min(40, country.TechnologyPercent + 0.2);
                    }
                }
                break;
                
            case StrategicObjective.MilitaryModernization:
                country.MilitarySpending = Math.Min(8, country.MilitarySpending + 0.2);
                if (country.TechnologyPercent < 40)
                    country.TechnologyPercent = Math.Min(40, country.TechnologyPercent + 0.15);
                break;
        }
        
        double threatLevel = CalculateRegionalThreatLevel(country);
        if (threatLevel > 0.6)
        {
            country.MilitarySpending = Math.Min(15, country.MilitarySpending + 0.5);
        }
    }

    private double CalculateRegionalThreatLevel(Country country)
    {
        double threatLevel = 0;
        
        var regionalEnemies = _simulation.Countries
            .Where(c => c.Region == country.Region && country.Enemies.Contains(c.Id))
            .ToList();
        
        foreach (var enemy in regionalEnemies)
        {
            double enemyStrength = enemy.MilitaryPower / (double)country.MilitaryPower;
            threatLevel += Math.Min(1.0, enemyStrength);
        }
        
        return Math.Min(1.0, threatLevel);
    }

    private void HandleResourceCompetition(Country country)
    {
        foreach (var resource in country.StrategicResources)
        {
            var producers = _simulation.CountryResources
                .Where(r => r.ResourceId == resource.Key && r.CountryId != country.Id)
                .ToList();

            foreach (var producer in producers)
            {
                var producerCountry = _simulation.GetCountry(producer.CountryId);
                if (producerCountry == null) continue;

                if (!country.Allies.Contains(producerCountry.Id))
                {
                    double competition = _random.NextDouble() * country.HegemonyDesire;
                    if (competition > 0.5 && producer.Surplus > 10)
                    {
                        if (country.DiplomaticRelations[producerCountry.Id] > -30)
                        {
                            country.DiplomaticRelations[producerCountry.Id] -= 5;
                            
                            if (country.DiplomaticRelations[producerCountry.Id] < -50)
                            {
                                InitiateResourceDiplomacy(country, producerCountry, resource.Key);
                            }
                        }
                    }
                }
            }
        }
    }

    private void InitiateResourceDiplomacy(Country country, Country target, string resource)
    {
        if (_random.NextDouble() < 0.3)
        {
            country.DiplomaticRelations[target.Id] += 10;
            
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = country.Id,
                CountryName = country.Name,
                Type = "Economic",
                Title = "Resource Diplomacy",
                Description = $"{country.Name} seeks closer ties with {target.Name} to secure {resource} supplies.",
                ImpactGdp = 1
            });
        }
    }

    private void EvaluateAlliances(Country country)
    {
        if (!_allianceStrategies.ContainsKey(country.Id))
        {
            _allianceStrategies[country.Id] = new AllianceStrategy
            {
                CountryId = country.Id,
                PreferredPartners = new List<string>(),
                AvoidedCountries = new List<string>()
            };
        }
        
        var strategy = _allianceStrategies[country.Id];
        
        foreach (var allyId in country.Allies.ToList())
        {
            var ally = _simulation.GetCountry(allyId);
            if (ally == null) continue;

            if (country.DiplomaticRelations[allyId] < 30)
            {
                if (_random.NextDouble() < 0.1)
                {
                    country.Allies.Remove(allyId);
                    ally.Allies.Remove(country.Id);
                }
            }
            
            if (country.MemoryOfConflicts.ContainsKey(allyId))
            {
                country.MemoryOfConflicts[allyId] = Math.Max(0, country.MemoryOfConflicts[allyId] - 1);
            }
        }

        if (country.HegemonyDesire > 0.5 || strategy.PreferredPartners.Count < 2)
        {
            var potentialAllies = _simulation.Countries
                .Where(c => c.Region == country.Region && 
                           c.Id != country.Id && 
                           !country.Allies.Contains(c.Id) &&
                           !country.Enemies.Contains(c.Id) &&
                           country.DiplomaticRelations[c.Id] > 50)
                .OrderByDescending(c => CalculateAllianceValue(country, c))
                .Take(3);

            foreach (var potential in potentialAllies)
            {
                if (_random.NextDouble() < 0.1 * _settings.AiAggressiveness)
                {
                    country.Allies.Add(potential.Id);
                    potential.Allies.Add(country.Id);
                    strategy.PreferredPartners.Add(potential.Id);
                    
                    _simulation.Events.Add(new SimulationEvent
                    {
                        Turn = _simulation.CurrentTurn,
                        CountryId = country.Id,
                        CountryName = country.Name,
                        Type = "Diplomatic",
                        Title = "Alliance Formed",
                        Description = $"{country.Name} and {potential.Name} have formed a strategic alliance.",
                        ImpactStability = 3
                    });
                }
            }
        }
    }

    private double CalculateAllianceValue(Country country, Country potential)
    {
        double value = 0;
        
        value += potential.MilitaryPower / 100.0;
        value += potential.Gdp / 1000000.0;
        
        if (potential.Region == country.Region)
            value *= 1.5;
        
        if (potential.StrategicResources.Keys.Any(r => country.StrategicResources.ContainsKey(r)))
            value *= 1.2;
        
        if (potential.DiplomaticRelations.TryGetValue(country.Id, out var relation))
            value += relation / 50.0;
        
        return value;
    }

    private void ProcessHistoricalMemory(Country country)
    {
        foreach (var memory in country.MemoryOfConflicts.ToList())
        {
            var otherCountry = _simulation.GetCountry(memory.Key);
            if (otherCountry == null) continue;

            if (memory.Value > 0)
            {
                int turnSinceConflict = _simulation.CurrentTurn - (country.LastWarTurn > 0 ? country.LastWarTurn : _simulation.CurrentTurn);
                if (turnSinceConflict > 20)
                {
                    country.MemoryOfConflicts[memory.Key] = Math.Max(0, memory.Value - 1);
                }
            }
        }

        foreach (var otherCountry in _simulation.Countries.Where(c => c.Id != country.Id))
        {
            if (!country.MemoryOfConflicts.ContainsKey(otherCountry.Id))
                country.MemoryOfConflicts[otherCountry.Id] = 0;
        }
    }

    private void PursueRegionalInfluence(Country country)
    {
        var goal = _strategicGoals[country.Id];
        
        if (goal.PrimaryObjective == StrategicObjective.RegionalHegemony || 
            goal.PrimaryObjective == StrategicObjective.RegionalInfluence)
        {
            var smallerNeighbors = _simulation.Countries
                .Where(c => c.Region == country.Region && 
                           c.Id != country.Id &&
                           c.MilitaryPower < country.MilitaryPower * 0.5 &&
                           !country.Allies.Contains(c.Id))
                .OrderBy(c => c.MilitaryPower)
                .ToList();
            
            foreach (var neighbor in smallerNeighbors.Take(2))
            {
                if (country.DiplomaticRelations[neighbor.Id] > 30 && _random.NextDouble() < 0.05)
                {
                    if (!country.Allies.Contains(neighbor.Id))
                    {
                        country.Allies.Add(neighbor.Id);
                        neighbor.Allies.Add(country.Id);
                        
                        _simulation.Events.Add(new SimulationEvent
                        {
                            Turn = _simulation.CurrentTurn,
                            CountryId = country.Id,
                            CountryName = country.Name,
                            Type = "Diplomatic",
                            Title = "Regional Influence",
                            Description = $"{country.Name} extends influence over {neighbor.Name} through diplomatic means.",
                            ImpactDiplomatic = 5
                        });
                    }
                }
            }
        }
        
        if (country.MilitaryPower > _simulation.Countries
            .Where(c => c.Region == country.Region && c.Id != country.Id)
            .Select(c => c.MilitaryPower)
            .DefaultIfEmpty(0)
            .Max())
        {
            country.RegionalPower = "Regional Leader";
        }
    }
}

public enum StrategicObjective
{
    EconomicGrowth,
    EconomicRecovery,
    Stabilization,
    RegionalHegemony,
    RegionalInfluence,
    AllianceBuilding,
    TechnologicalAdvancement,
    TradeSurplus,
    MilitaryModernization,
    EducationReform
}

public class StrategicGoal
{
    public string CountryId { get; set; } = string.Empty;
    public StrategicObjective PrimaryObjective { get; set; }
    public List<StrategicObjective> SecondaryObjectives { get; set; } = new();
    public int TimeHorizon { get; set; }
    public double Progress { get; set; }
    public Dictionary<string, double> ProgressMetrics { get; set; } = new();
}

public class AllianceStrategy
{
    public string CountryId { get; set; } = string.Empty;
    public List<string> PreferredPartners { get; set; } = new();
    public List<string> AvoidedCountries { get; set; } = new();
    public double TrustThreshold { get; set; } = 50;
}
