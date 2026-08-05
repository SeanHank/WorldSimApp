using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;

namespace WorldSimApp.Simulation;

public class EventSystem
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;
    private readonly GameSettings _settings;

    private EconomicCycle _globalEconomicCycle = new();
    private List<ChainEvent> _chainEvents = new();
    private List<ClimateEvent> _climateEvents = new();
    private List<Technology> _technologies = new();
    
    private Dictionary<string, List<CountrySpecificEvent>> _countrySpecificEvents = new();
    private HashSet<string> _triggeredCountryEvents = new();

    public EventSystem(WorldSimulation simulation, GameSettings settings)
    {
        _simulation = simulation;
        _settings = settings;
        _random = new Random();
        InitializeTechnologies();
        InitializeChainEvents();
        InitializeCountrySpecificEvents();
    }

    private void InitializeCountrySpecificEvents()
    {
        _countrySpecificEvents = new Dictionary<string, List<CountrySpecificEvent>>
        {
            ["USA"] = new List<CountrySpecificEvent>
            {
                new() { Id = "tech_boom", Title = "Technology Boom", Description = "Silicon Valley revolution transforms the economy", TriggerCondition = "TechnologyPercent>30", ImpactGdp = 4, ImpactHappiness = 3, OneTime = true },
                new() { Id = "housing_bubble", Title = "Housing Bubble Burst", Description = "Real estate market collapses", TriggerCondition = "EconomicGrowth>5", ImpactGdp = -5, ImpactStability = -8, OneTime = true },
                new() { Id = "fracking_revolution", Title = "Fracking Revolution", Description = "Energy independence achieved", TriggerCondition = "Region=North America", ImpactGdp = 2, ImpactHappiness = 2, OneTime = true },
                new() { Id = "trade_war_china", Title = "Trade War with Eastern Republic", Description = "Escalating tariffs affect global trade", TriggerCondition = "DiplomaticRelation<=-30", ImpactGdp = -2, ImpactStability = -3, OneTime = true },
                new() { Id = "cold_war_end", Title = "Cold War Ends", Description = "Geopolitical landscape shifts dramatically", TriggerCondition = "MilitaryPower>1000", ImpactGdp = 1, ImpactStability = 5, OneTime = true }
            },
            ["CHN"] = new List<CountrySpecificEvent>
            {
                new() { Id = "economic_reform", Title = "Economic Reform Era", Description = "Market reforms accelerate growth", TriggerCondition = "EconomicGrowth<3", ImpactGdp = 5, ImpactHappiness = 3, OneTime = true },
                new() { Id = "加入WTO", Title = "WTO Accession", Description = "Eastern Republic joins World Trade Organization", TriggerCondition = "Turn>10", ImpactGdp = 4, ImpactStability = 3, OneTime = true },
                new() { Id = "one_belt_one_road", Title = "Belt and Road Initiative", Description = "New silk road transforms trade", TriggerCondition = "Gdp>1000000", ImpactGdp = 3, ImpactMilitary = 5, OneTime = true },
                new() { Id = "tech_competition", Title = "Technology Competition", Description = "Rivalry with Atlantis Federation intensifies", TriggerCondition = "TechnologyPercent>25", ImpactGdp = 2, ImpactStability = -2, OneTime = true },
                new() { Id = "demographic_crisis", Title = "Demographic Crisis", Description = "Aging population strains economy", TriggerCondition = "MedianAge>40", ImpactGdp = -3, ImpactStability = -2, OneTime = true }
            },
            ["DEU"] = new List<CountrySpecificEvent>
            {
                new() { Id = "reunification", Title = "Reunification Effects", Description = "East-West integration challenges", TriggerCondition = "Region=Europe", ImpactGdp = -2, ImpactStability = -3, OneTime = true },
                new() { Id = "euro_crisis", Title = "Euro Crisis", Description = "European debt crisis affects Valoria", TriggerCondition = "Stability<60", ImpactGdp = -3, ImpactStability = -5, OneTime = true },
                new() { Id = "energy_transition", Title = "Energy Transition", Description = "Renewable energy revolution", TriggerCondition = "TechnologyPercent>20", ImpactGdp = 1, ImpactHappiness = 4, OneTime = true },
                new() { Id = "automotive_crisis", Title = "Automotive Industry Crisis", Description = "EV transition disrupts manufacturing", TriggerCondition = "ManufacturingPercent>30", ImpactGdp = -2, ImpactStability = -2, OneTime = true }
            },
            ["RUS"] = new List<CountrySpecificEvent>
            {
                new() { Id = "soviet_collapse", Title = "Soviet Legacy", Description = "Dealing with Northern Empire past", TriggerCondition = "Turn>5", ImpactStability = -2, ImpactHappiness = -3, OneTime = true },
                new() { Id = "sanctions_crisis", Title = "Western Sanctions", Description = "Economic pressure intensifies", TriggerCondition = "DiplomaticRelation<=-50", ImpactGdp = -4, ImpactStability = -3, OneTime = true },
                new() { Id = "energy_leverage", Title = "Energy Leverage", Description = "Resource diplomacy gains influence", TriggerCondition = "Region=Europe", ImpactGdp = 3, ImpactMilitary = 2, OneTime = true },
                new() { Id = "military_modernization", Title = "Military Modernization", Description = "Armed forces undergo transformation", TriggerCondition = "MilitarySpending>3", ImpactMilitary = 15, ImpactGdp = -1, OneTime = true }
            },
            ["GBR"] = new List<CountrySpecificEvent>
            {
                new() { Id = "brexit", Title = "Brexit Effects", Description = "Continental Union departure reshapes economy", TriggerCondition = "Region=Europe", ImpactGdp = -2, ImpactStability = -4, OneTime = true },
                new() { Id = "financial_hub", Title = "Financial Hub Status", Description = "City maintains influence", TriggerCondition = "Gdp>500000", ImpactGdp = 2, ImpactStability = 2, OneTime = true },
                new() { Id = "commonwealth_ties", Title = "Commonwealth Relations", Description = "Former colonies maintain ties", TriggerCondition = "Region=Europe", ImpactDiplomatic = 10, OneTime = true }
            },
            ["JPN"] = new List<CountrySpecificEvent>
            {
                new() { Id = "lost_decade", Title = "Economic Stagnation", Description = "Prolonged recession challenges economy", TriggerCondition = "EconomicGrowth>1", ImpactGdp = -3, ImpactStability = -3, OneTime = true },
                new() { Id = "aging_crisis", Title = "Aging Society Crisis", Description = "Demographic challenges intensify", TriggerCondition = "MedianAge>45", ImpactGdp = -2, ImpactStability = -2, OneTime = true },
                new() { Id = "tech_innovation", Title = "Tech Innovation Wave", Description = "Robotics and automation advance", TriggerCondition = "TechnologyPercent>25", ImpactGdp = 3, ImpactHappiness = 2, OneTime = true },
                new() { Id = "natural_disaster", Title = "Major Earthquake", Description = "Natural disaster devastates region", TriggerCondition = "Region=Asia", ImpactGdp = -4, ImpactStability = -5, OneTime = true }
            },
            ["IND"] = new List<CountrySpecificEvent>
            {
                new() { Id = "economic_boom", Title = "Economic Boom", Description = "Rapid growth transforms nation", TriggerCondition = "EconomicGrowth>7", ImpactGdp = 5, ImpactHappiness = 4, OneTime = true },
                new() { Id = "it_revolution", Title = "IT Revolution", Description = "Tech sector dominates global services", TriggerCondition = "TechnologyPercent>15", ImpactGdp = 3, ImpactStability = 2, OneTime = true },
                new() { Id = "regional_power", Title = "Regional Power Rise", Description = "Republic of Vishuna emerges as regional leader", TriggerCondition = "MilitaryPower>300", ImpactMilitary = 10, ImpactDiplomatic = 15, OneTime = true }
            },
            ["BRA"] = new List<CountrySpecificEvent>
            {
                new() { Id = "commodity_boom", Title = "Commodity Super-Cycle", Description = "Natural resources drive growth", TriggerCondition = "Region=South America", ImpactGdp = 4, ImpactHappiness = 2, OneTime = true },
                new() { Id = "political_crisis", Title = "Political Crisis", Description = "Corruption scandal rocks nation", TriggerCondition = "Stability<50", ImpactGdp = -3, ImpactStability = -6, OneTime = true },
                new() { Id = "deforestation_crisis", Title = "Amazon Crisis", Description = "Environmental concerns escalate", TriggerCondition = "Region=South America", ImpactGdp = -1, ImpactHappiness = -4, OneTime = true }
            },
            ["IRN"] = new List<CountrySpecificEvent>
            {
                new() { Id = "nuclear_deal", Title = "Nuclear Agreement", Description = "International relations shift", TriggerCondition = "DiplomaticRelation>30", ImpactGdp = 3, ImpactStability = 4, OneTime = true },
                new() { Id = "regional_hegemony", Title = "Regional Influence", Description = "Middle East power expands", TriggerCondition = "MilitaryPower>200", ImpactMilitary = 8, ImpactDiplomatic = 5, OneTime = true },
                new() { Id = "sanctions_impact", Title = "Economic Isolation", Description = "International sanctions bite", TriggerCondition = "SanctionedBy.Count>2", ImpactGdp = -4, ImpactHappiness = -3, OneTime = true }
            },
            ["SAU"] = new List<CountrySpecificEvent>
            {
                new() { Id = "oil_boom", Title = "Oil Price Surge", Description = "Energy revenues flood economy", TriggerCondition = "Region=Middle East", ImpactGdp = 5, ImpactHappiness = 5, OneTime = true },
                new() { Id = "vision_2030", Title = "Vision 2030", Description = "Economic diversification begins", TriggerCondition = "Gdp>300000", ImpactGdp = 2, ImpactStability = 3, OneTime = true },
                new() { Id = "regional_rivalry", Title = "Regional Rivalry", Description = "Tensions with Persian Republic escalate", TriggerCondition = "Region=Middle East", ImpactMilitary = 5, ImpactStability = -2, OneTime = true }
            }
        };
    }

    private void InitializeTechnologies()
    {
        _technologies = new List<Technology>
        {
            new() { Id = "internet", Name = "Internet", Category = "Communication", GdpBonus = 2.0, MilitaryBonus = 0.5, ResearchCost = 50 },
            new() { Id = "ai", Name = "Artificial Intelligence", Category = "Technology", GdpBonus = 3.0, MilitaryBonus = 2.0, ResearchCost = 100 },
            new() { Id = "nuclear", Name = "Nuclear Power", Category = "Energy", GdpBonus = 1.5, MilitaryBonus = 3.0, ResearchCost = 80 },
            new() { Id = "space", Name = "Space Technology", Category = "Research", GdpBonus = 1.0, MilitaryBonus = 2.5, ResearchCost = 120 },
            new() { Id = "quantum", Name = "Quantum Computing", Category = "Technology", GdpBonus = 4.0, MilitaryBonus = 3.0, ResearchCost = 150 },
            new() { Id = "renewable", Name = "Renewable Energy", Category = "Energy", GdpBonus = 2.0, HappinessBonus = 1.0, ResearchCost = 60 }
        };
    }

    private void InitializeChainEvents()
    {
        _chainEvents = new List<ChainEvent>
        {
            new() { Id = "crash_recession", TriggerEventId = "market_crash", FollowUpEventId = "recession", TriggerProbability = 0.7, DelayTurns = 2 },
            new() { Id = "war_refugees", TriggerEventId = "war_start", FollowUpEventId = "refugee_crisis", TriggerProbability = 0.6, DelayTurns = 1 },
            new() { Id = "drought_famine", TriggerEventId = "severe_drought", FollowUpEventId = "famine", TriggerProbability = 0.5, DelayTurns = 3 },
            new() { Id = "tech_boom_economy", TriggerEventId = "major_tech_breakthrough", FollowUpEventId = "economic_boom", TriggerProbability = 0.8, DelayTurns = 1 }
        };
    }

    public void GenerateEnhancedEvents()
    {
        UpdateEconomicCycle();
        CheckFinancialCrisis();
        GenerateCountrySpecificEvents();
        GenerateRandomEvents();
        CheckChainEvents();
        CheckTechnologyBreakthroughs();
        SimulateClimateEvents();
    }

    private void GenerateCountrySpecificEvents()
    {
        foreach (var country in _simulation.Countries)
        {
            if (!_countrySpecificEvents.ContainsKey(country.Id))
            {
                var genericEvents = new List<CountrySpecificEvent>
                {
                    new() { Id = "regional_conflict", Title = "Regional Conflict Escalates", Description = "Tensions in the region intensify", TriggerCondition = "Stability<40", ImpactStability = -4, ImpactMilitary = 3, OneTime = false },
                    new() { Id = "trade_dispute", Title = "Trade Dispute", Description = "Economic tensions arise", TriggerCondition = "Gdp>100000", ImpactGdp = -1, ImpactDiplomatic = -3, OneTime = false },
                    new() { Id = "cultural_soft_power", Title = "Cultural Soft Power", Description = "Culture gains global influence", TriggerCondition = "EducationLevel>60", ImpactHappiness = 2, ImpactDiplomatic = 3, OneTime = false }
                };
                CheckAndTriggerEvents(country, genericEvents);
                continue;
            }
            
            CheckAndTriggerEvents(country, _countrySpecificEvents[country.Id]);
        }
    }

    private void CheckAndTriggerEvents(Country country, List<CountrySpecificEvent> events)
    {
        foreach (var evt in events)
        {
            string eventKey = $"{country.Id}_{evt.Id}";
            
            if (evt.OneTime && _triggeredCountryEvents.Contains(eventKey))
                continue;
            
            if (EvaluateTriggerCondition(country, evt.TriggerCondition))
            {
                if (_random.NextDouble() < 0.15)
                {
                    TriggerCountrySpecificEvent(country, evt);
                    
                    if (evt.OneTime)
                    {
                        _triggeredCountryEvents.Add(eventKey);
                    }
                }
            }
        }
    }

    private bool EvaluateTriggerCondition(Country country, string condition)
    {
        try
        {
            if (condition.Contains(">"))
            {
                var parts = condition.Split(">");
                var propName = parts[0].Trim();
                var value = double.Parse(parts[1].Trim());
                return GetPropertyValue(country, propName) > value;
            }
            else if (condition.Contains("<"))
            {
                var parts = condition.Split("<");
                var propName = parts[0].Trim();
                var value = double.Parse(parts[1].Trim());
                return GetPropertyValue(country, propName) < value;
            }
            else if (condition.Contains("="))
            {
                var parts = condition.Split("=");
                var propName = parts[0].Trim();
                var value = parts[1].Trim();
                return GetPropertyStringValue(country, propName) == value;
            }
        }
        catch { }
        
        return false;
    }

    private double GetPropertyValue(Country country, string propName)
    {
        return propName switch
        {
            "TechnologyPercent" => country.TechnologyPercent,
            "EconomicGrowth" => country.EconomicGrowth,
            "Gdp" => country.Gdp,
            "Stability" => country.Stability,
            "MilitaryPower" => country.MilitaryPower,
            "MilitarySpending" => country.MilitarySpending,
            "MedianAge" => country.MedianAge,
            "Turn" => _simulation.CurrentTurn,
            _ => 0
        };
    }

    private string GetPropertyStringValue(Country country, string propName)
    {
        return propName switch
        {
            "Region" => country.Region,
            _ => ""
        };
    }

    private void TriggerCountrySpecificEvent(Country country, CountrySpecificEvent evt)
    {
        country.Gdp *= (1 + evt.ImpactGdp / 100);
        country.Stability = Math.Clamp(country.Stability + evt.ImpactStability, 0, 100);
        country.Happiness = Math.Clamp(country.Happiness + evt.ImpactHappiness, 0, 100);
        country.MilitaryPower = (int)(country.MilitaryPower * (1 + evt.ImpactMilitary / 100));
        
        _simulation.Events.Add(new SimulationEvent
        {
            Turn = _simulation.CurrentTurn,
            CountryId = country.Id,
            CountryName = country.Name,
            Type = "CountrySpecific",
            Title = evt.Title,
            Description = evt.Description,
            ImpactGdp = evt.ImpactGdp,
            ImpactStability = evt.ImpactStability,
            ImpactHappiness = evt.ImpactHappiness,
            ImpactMilitary = evt.ImpactMilitary
        });
    }

    private void UpdateEconomicCycle()
    {
        _globalEconomicCycle.DurationInPhase++;

        switch (_globalEconomicCycle.Phase)
        {
            case EconomicCyclePhase.Recovery:
                if (_globalEconomicCycle.DurationInPhase > 4)
                {
                    _globalEconomicCycle.Phase = EconomicCyclePhase.Expansion;
                    _globalEconomicCycle.DurationInPhase = 0;
                    _globalEconomicCycle.PhaseMultiplier = 1.2;
                }
                break;

            case EconomicCyclePhase.Expansion:
                _globalEconomicCycle.CrisisProbability = 0.05;
                if (_globalEconomicCycle.DurationInPhase > 6 || _random.NextDouble() < _globalEconomicCycle.CrisisProbability)
                {
                    _globalEconomicCycle.Phase = EconomicCyclePhase.Peak;
                    _globalEconomicCycle.DurationInPhase = 0;
                }
                break;

            case EconomicCyclePhase.Peak:
                if (_random.NextDouble() < 0.3)
                {
                    _globalEconomicCycle.Phase = EconomicCyclePhase.Recession;
                    _globalEconomicCycle.DurationInPhase = 0;
                    _globalEconomicCycle.PhaseMultiplier = 0.7;
                    TriggerRecessionEvent();
                }
                break;

            case EconomicCyclePhase.Recession:
                if (_globalEconomicCycle.DurationInPhase > 5)
                {
                    _globalEconomicCycle.Phase = EconomicCyclePhase.Recovery;
                    _globalEconomicCycle.DurationInPhase = 0;
                }
                break;
        }

        foreach (var country in _simulation.Countries)
        {
            double cycleEffect = (_globalEconomicCycle.PhaseMultiplier - 1.0) * country.EconomicGrowth;
            country.EconomicGrowth += cycleEffect;
        }
    }

    private void TriggerRecessionEvent()
    {
        _simulation.Events.Add(new SimulationEvent
        {
            Turn = _simulation.CurrentTurn,
            CountryId = "WORLD",
            CountryName = "Global Economy",
            Type = "Economic",
            Title = "Global Recession Begins",
            Description = "The global economy has entered a recessionary period. Economic growth worldwide is slowing down.",
            ImpactGdp = -5,
            ImpactStability = -3
        });
    }

    private void CheckFinancialCrisis()
    {
        if (_random.NextDouble() < 0.02 && !_globalEconomicCycle.IsInCrisis)
        {
            var crisisType = _random.NextDouble() < 0.5 ? "Stock Market Crash" : "Banking Crisis";
            _globalEconomicCycle.IsInCrisis = true;
            _globalEconomicCycle.CrisisType = crisisType;
            _globalEconomicCycle.CrisisDuration = 5;

            foreach (var country in _simulation.Countries)
            {
                country.Gdp *= 0.9;
                country.Inflation += 3;

                _simulation.Events.Add(new SimulationEvent
                {
                    Turn = _simulation.CurrentTurn,
                    CountryId = country.Id,
                    CountryName = country.Name,
                    Type = "Economic",
                    Title = $"{crisisType}!",
                    Description = $"A {crisisType.ToLower()} has struck {country.Name}, causing significant economic damage.",
                    ImpactGdp = -8,
                    ImpactStability = -5
                });
            }
        }

        if (_globalEconomicCycle.IsInCrisis)
        {
            _globalEconomicCycle.CrisisDuration--;
            if (_globalEconomicCycle.CrisisDuration <= 0)
            {
                _globalEconomicCycle.IsInCrisis = false;
                _globalEconomicCycle.CrisisType = string.Empty;
            }
        }
    }

    private void GenerateRandomEvents()
    {
        if (_simulation.Countries.Count == 0) return;

        int eventsThisTurn = (int)((_random.Next(2, 5) * _settings.EventFrequency));

        for (int i = 0; i < eventsThisTurn; i++)
        {
            var country = _simulation.Countries[_random.Next(_simulation.Countries.Count)];
            var eventType = GetWeightedEventType();
            
            if (eventType == "Natural Disaster" && _random.NextDouble() > _settings.DisasterFrequency)
                continue;
            
            if (eventType == "Military" && _random.NextDouble() > _settings.WarProbability)
                continue;

            var evt = CreateEnhancedEvent(country, eventType);
            if (evt != null)
            {
                ApplyEventEffects(evt);
                _simulation.Events.Add(evt);
            }
        }
    }

    private string GetWeightedEventType()
    {
        double roll = _random.NextDouble();
        
        if (_globalEconomicCycle.Phase == EconomicCyclePhase.Recession || _globalEconomicCycle.IsInCrisis)
        {
            if (roll < 0.3) return "Economic Crisis";
            if (roll < 0.5) return "Economic";
        }
        
        if (roll < 0.25) return "Economic";
        if (roll < 0.40) return "Military";
        if (roll < 0.55) return "Political";
        if (roll < 0.70) return "Social";
        if (roll < 0.85) return "Diplomatic";
        return "Natural Disaster";
    }

    private SimulationEvent? CreateEnhancedEvent(Country country, string eventType)
    {
        string title;
        string description;
        double impactGdp = 0, impactStability = 0, impactMilitary = 0, impactHappiness = 0;

        switch (eventType)
        {
            case "Economic":
            case "Economic Crisis":
                title = GetEconomicEventTitle();
                impactGdp = (_random.NextDouble() - 0.3) * 8;
                if (eventType == "Economic Crisis") impactGdp *= 1.5;
                impactStability = (_random.NextDouble() - 0.5) * 5;
                impactHappiness = impactGdp * 0.5;
                description = $"{title} in {country.Name}.";
                break;

            case "Military":
                title = GetMilitaryEventTitle();
                impactMilitary = (_random.NextDouble() - 0.3) * 10;
                impactStability = (_random.NextDouble() - 0.5) * 5;
                description = $"{country.Name} has {title.ToLower()}.";
                break;

            case "Political":
                title = GetPoliticalEventTitle();
                impactStability = (_random.NextDouble() - 0.5) * 15;
                impactHappiness = (_random.NextDouble() - 0.5) * 10;
                description = $"{title} in {country.Name}.";
                break;

            case "Social":
                title = GetSocialEventTitle();
                impactHappiness = (_random.NextDouble() - 0.5) * 12;
                impactStability = (_random.NextDouble() - 0.5) * 8;
                description = $"{title} occurred in {country.Name}.";
                break;

            case "Diplomatic":
                title = GetDiplomaticEventTitle();
                var otherCountry = _simulation.Countries.FirstOrDefault(c => c.Id != country.Id);
                if (otherCountry != null && _random.NextDouble() > 0.5)
                {
                    if (!country.Allies.Contains(otherCountry.Id))
                    {
                        country.Allies.Add(otherCountry.Id);
                        otherCountry.Allies.Add(country.Id);
                    }
                    description = $"{country.Name} and {otherCountry.Name} have {title.ToLower()}.";
                }
                else
                {
                    description = $"{country.Name} has {title.ToLower()}.";
                }
                impactStability = (_random.NextDouble() - 0.5) * 5;
                break;

            case "Natural Disaster":
                title = GetDisasterEventTitle();
                impactGdp = -_random.NextDouble() * 8 - 2;
                impactStability = -_random.NextDouble() * 10;
                impactHappiness = -_random.NextDouble() * 15;
                description = $"A {title} has struck {country.Name}, causing significant damage.";
                break;

            default:
                return null;
        }

        return new SimulationEvent
        {
            Turn = _simulation.CurrentTurn,
            CountryId = country.Id,
            CountryName = country.Name,
            Type = eventType,
            Title = title,
            Description = description,
            ImpactGdp = impactGdp,
            ImpactStability = impactStability,
            ImpactMilitary = impactMilitary,
            ImpactHappiness = impactHappiness
        };
    }

    private string GetEconomicEventTitle()
    {
        var titles = new[] { "Trade Agreement Signed", "Market Crash", "Tech Boom", "Recession", 
            "Export Surge", "Currency Reform", "Investment Wave", "Bank Bailout", "Trade War" };
        return titles[_random.Next(titles.Length)];
    }

    private string GetMilitaryEventTitle()
    {
        var titles = new[] { "Military Exercise", "Weapons Test", "Troop Mobilization", 
            "Defense Upgrade", "Conflict Escalation", "Naval Patrol", "Drone Strike" };
        return titles[_random.Next(titles.Length)];
    }

    private string GetPoliticalEventTitle()
    {
        var titles = new[] { "Election Results", "Policy Change", "Scandal Uncovered", 
            "Reform Announced", "Coup Attempt", "Summit Meeting", "Constitutional Crisis" };
        return titles[_random.Next(titles.Length)];
    }

    private string GetSocialEventTitle()
    {
        var titles = new[] { "Protest Broke Out", "Cultural Festival", "Immigration Wave", 
            "Education Reform", "Healthcare Crisis", "Labor Strike", "Religious Movement" };
        return titles[_random.Next(titles.Length)];
    }

    private string GetDiplomaticEventTitle()
    {
        var titles = new[] { "Alliance Formed", "Sanctions Implemented", "Peace Treaty", 
            "Embassy Opened", "Tensions Rise", "Trade Deal", "Diplomatic Crisis" };
        return titles[_random.Next(titles.Length)];
    }

    private string GetDisasterEventTitle()
    {
        var titles = new[] { "Earthquake", "Flood", "Pandemic", "Drought", "Hurricane", "Wildfire", "Tsunami" };
        return titles[_random.Next(titles.Length)];
    }

    private void ApplyEventEffects(SimulationEvent evt)
    {
        var country = _simulation.Countries.FirstOrDefault(c => c.Id == evt.CountryId);
        if (country == null) return;

        if (evt.Type == "Economic Crisis")
        {
            country.Gdp *= (1 + evt.ImpactGdp / 100);
            country.Inflation += Math.Abs(evt.ImpactGdp) * 0.5;
        }
        else
        {
            country.Gdp *= (1 + evt.ImpactGdp / 100);
        }
        
        country.Stability = Math.Clamp(country.Stability + evt.ImpactStability, 0, 100);
        country.MilitaryPower = Math.Max(1, (int)(country.MilitaryPower * (1 + evt.ImpactMilitary / 100)));
        country.Happiness = Math.Clamp(country.Happiness + evt.ImpactHappiness, 0, 100);

        if (evt.Title.Contains("War"))
        {
            CheckChainEvents();
        }
    }

    private void CheckChainEvents()
    {
        foreach (var chainEvent in _chainEvents.Where(e => !e.HasTriggered))
        {
            var recentEvents = _simulation.Events.Where(e => 
                e.Title.ToLower().Contains(chainEvent.TriggerEventId.Replace("_", " ")));

            if (recentEvents.Any() && _random.NextDouble() < chainEvent.TriggerProbability)
            {
                chainEvent.HasTriggered = true;
                TriggerChainEvent(chainEvent);
            }
        }

        foreach (var chainEvent in _chainEvents.Where(e => e.HasTriggered))
        {
            chainEvent.HasTriggered = false;
        }
    }

    private void TriggerChainEvent(ChainEvent chainEvent)
    {
        var country = _simulation.Countries[_random.Next(_simulation.Countries.Count)];
        
        _simulation.Events.Add(new SimulationEvent
        {
            Turn = _simulation.CurrentTurn,
            CountryId = country.Id,
            CountryName = country.Name,
            Type = "Chain Event",
            Title = FormatChainEventTitle(chainEvent.FollowUpEventId),
            Description = $"Following recent events, {country.Name} now faces {chainEvent.FollowUpEventId.Replace("_", " ")}.",
            ImpactGdp = -3,
            ImpactStability = -2
        });
    }

    private string FormatChainEventTitle(string eventId)
    {
        return eventId switch
        {
            "recession" => "Economic Recession",
            "refugee_crisis" => "Refugee Crisis",
            "famine" => "Famine",
            "economic_boom" => "Economic Boom",
            _ => eventId.Replace("_", " ")
        };
    }

    private void CheckTechnologyBreakthroughs()
    {
        if (_random.NextDouble() < 0.03)
        {
            var undiscovered = _technologies.Where(t => !t.IsDiscovered).ToList();
            if (undiscovered.Count > 0)
            {
                var tech = undiscovered[_random.Next(undiscovered.Count)];
                tech.IsDiscovered = true;
                tech.TurnDiscovered = _simulation.CurrentTurn;

                foreach (var country in _simulation.Countries.Where(c => c.TechnologyPercent > 20))
                {
                    country.Gdp *= (1 + tech.GdpBonus / 100);
                    country.MilitaryPower = (int)(country.MilitaryPower * (1 + tech.MilitaryBonus / 100));
                }

                _simulation.Events.Add(new SimulationEvent
                {
                    Turn = _simulation.CurrentTurn,
                    CountryId = "WORLD",
                    CountryName = "Global",
                    Type = "Technology",
                    Title = $"Technology Breakthrough: {tech.Name}",
                    Description = $"A major breakthrough in {tech.Name.ToLower()} has been achieved, transforming the global economy.",
                    ImpactGdp = tech.GdpBonus
                });
            }
        }
    }

    private void SimulateClimateEvents()
    {
        if (_random.NextDouble() < 0.01)
        {
            var climateEvent = new ClimateEvent
            {
                Id = $"climate_{_simulation.CurrentTurn}",
                Type = _random.NextDouble() < 0.5 ? "Global Warming" : "Extreme Weather",
                TurnStart = _simulation.CurrentTurn,
                Duration = 10,
                GlobalTemperatureChange = _random.NextDouble() * 0.5,
                IsActive = true
            };

            _climateEvents.Add(climateEvent);

            foreach (var country in _simulation.Countries)
            {
                country.EconomicGrowth -= climateEvent.GlobalTemperatureChange * 0.5;
                if (country.Region == "Middle East" || country.Region == "Africa")
                {
                    country.EconomicGrowth -= 1;
                    country.Happiness -= 2;
                }
            }

            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = "WORLD",
                CountryName = "Global",
                Type = "Climate",
                Title = $"Climate Event: {climateEvent.Type}",
                Description = $"A significant {climateEvent.Type.ToLower()} event is affecting economies worldwide.",
                ImpactGdp = -1,
                ImpactHappiness = -2
            });
        }

        foreach (var evt in _climateEvents.Where(e => e.IsActive).ToList())
        {
            if (_simulation.CurrentTurn - evt.TurnStart > evt.Duration)
            {
                evt.IsActive = false;
            }
        }
    }

    public string GetEconomicCycleStatus()
    {
        return $"Global Economy: {_globalEconomicCycle.Phase} (Duration: {_globalEconomicCycle.DurationInPhase})";
    }
}

public class CountrySpecificEvent
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TriggerCondition { get; set; } = string.Empty;
    public double ImpactGdp { get; set; }
    public double ImpactStability { get; set; }
    public double ImpactHappiness { get; set; }
    public double ImpactMilitary { get; set; }
    public double ImpactDiplomatic { get; set; }
    public bool OneTime { get; set; }
}
