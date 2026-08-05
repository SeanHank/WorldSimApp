using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;

namespace WorldSimApp.Simulation;

public class EventGenerator
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;
    private readonly GameSettings _settings;

    private readonly string[] _eventTypes = { "Economic", "Military", "Political", "Social", "Diplomatic", "Natural Disaster" };
    private readonly string[] _economicTitles = { "Trade Agreement Signed", "Market Crash", "Tech Boom", "Recession", "Export Surge", "Currency Reform", "Investment Wave" };
    private readonly string[] _militaryTitles = { "Military Exercise", "Weapons Test", "Troop Mobilization", "Defense Upgrade", "Conflict Escalation", "Naval Patrol" };
    private readonly string[] _politicalTitles = { "Election Results", "Policy Change", "Scandal Uncovered", "Reform Announced", "Coup Attempt", "Summit Meeting" };
    private readonly string[] _socialTitles = { "Protest Broke Out", "Cultural Festival", "Immigration Wave", "Education Reform", "Healthcare Crisis", "Labor Strike" };
    private readonly string[] _diplomaticTitles = { "Alliance Formed", "Sanctions Implemented", "Peace Treaty", "Embassy Opened", "Tensions Rise", "Trade Deal" };
    private readonly string[] _disasterTitles = { "Earthquake", "Flood", "Pandemic", "Drought", "Hurricane", "Wildfire" };

    public EventGenerator(WorldSimulation simulation, GameSettings settings)
    {
        _simulation = simulation;
        _settings = settings;
        _random = new Random();
    }

    public void GenerateRandomEvents()
    {
        if (_simulation.Countries.Count == 0) return;
        
        int eventsThisTurn = (int)((_random.Next(2, 5) * _settings.EventFrequency));
        
        for (int i = 0; i < eventsThisTurn; i++)
        {
            var country = _simulation.Countries[_random.Next(_simulation.Countries.Count)];
            var eventType = _eventTypes[_random.Next(_eventTypes.Length)];
            
            if (eventType == "Natural Disaster" && _random.NextDouble() > _settings.DisasterFrequency)
                continue;
            
            if (eventType == "Military" && _random.NextDouble() > _settings.WarProbability)
                continue;
            
            var evt = CreateEvent(country, eventType);
            if (evt != null)
            {
                ApplyEventEffects(evt);
                _simulation.Events.Add(evt);
            }
        }
        
        GeneratePlayerDecision();
    }

    private void GeneratePlayerDecision()
    {
        if (string.IsNullOrEmpty(_simulation.PlayerCountryId)) return;
        
        var playerCountry = _simulation.Countries.FirstOrDefault(c => c.Id == _simulation.PlayerCountryId);
        if (playerCountry == null) return;
        
        if (_simulation.Decisions.Any(d => d.IsPlayerDecision && !d.IsResolved)) return;
        
        if (_random.NextDouble() < 0.2)
        {
            var options = GenerateDecisionOptions(playerCountry, DecisionType.Economic);
            options.AddRange(GenerateDecisionOptions(playerCountry, DecisionType.Domestic));
            
            if (options.Count > 0)
            {
                var decision = new DecisionEvent
                {
                    Turn = _simulation.CurrentTurn,
                    CountryId = playerCountry.Id,
                    Type = DecisionType.Economic,
                    Title = "Policy Decision Required",
                    Description = $"The government of {playerCountry.Name} must make an important decision.",
                    Options = options,
                    IsPlayerDecision = true,
                    IsResolved = false
                };
                
                _simulation.Decisions.Add(decision);
            }
        }
    }

    public SimulationEvent? CreateEvent(Country country, string eventType)
    {
        string title;
        string description;
        double impactGdp = 0, impactStability = 0, impactMilitary = 0, impactHappiness = 0;
        
        switch (eventType)
        {
            case "Economic":
                title = _economicTitles[_random.Next(_economicTitles.Length)];
                impactGdp = (_random.NextDouble() - 0.3) * 8;
                impactStability = (_random.NextDouble() - 0.5) * 5;
                impactHappiness = impactGdp * 0.5;
                description = GenerateEconomicDescription(title, country, impactGdp);
                break;
            case "Military":
                title = _militaryTitles[_random.Next(_militaryTitles.Length)];
                impactMilitary = (_random.NextDouble() - 0.3) * 10;
                impactStability = (_random.NextDouble() - 0.5) * 5;
                description = $"{country.Name} has {title.ToLower()}.";
                break;
            case "Political":
                title = _politicalTitles[_random.Next(_politicalTitles.Length)];
                impactStability = (_random.NextDouble() - 0.5) * 15;
                impactHappiness = (_random.NextDouble() - 0.5) * 10;
                description = $"{title} in {country.Name}.";
                break;
            case "Social":
                title = _socialTitles[_random.Next(_socialTitles.Length)];
                impactHappiness = (_random.NextDouble() - 0.5) * 12;
                impactStability = (_random.NextDouble() - 0.5) * 8;
                description = $"{title} occurred in {country.Name}.";
                break;
            case "Diplomatic":
                title = _diplomaticTitles[_random.Next(_diplomaticTitles.Length)];
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
                title = _disasterTitles[_random.Next(_disasterTitles.Length)];
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

    private string GenerateEconomicDescription(string title, Country country, double impact)
    {
        var impactStr = impact > 0 ? "positively" : "negatively";
        return $"{title} in {country.Name} has affected the economy {impactStr}.";
    }

    private void ApplyEventEffects(SimulationEvent evt)
    {
        var country = _simulation.Countries.FirstOrDefault(c => c.Id == evt.CountryId);
        if (country == null) return;
        
        country.Gdp *= (1 + evt.ImpactGdp / 100);
        country.Stability = Math.Clamp(country.Stability + evt.ImpactStability, 0, 100);
        country.MilitaryPower = Math.Max(1, (int)(country.MilitaryPower * (1 + evt.ImpactMilitary / 100)));
        country.Happiness = Math.Clamp(country.Happiness + evt.ImpactHappiness, 0, 100);
    }

    public List<DecisionOption> GenerateDecisionOptions(Country country, DecisionType type)
    {
        var options = new List<DecisionOption>();
        
        switch (type)
        {
            case DecisionType.Economic:
                options.Add(new DecisionOption
                {
                    Id = "invest",
                    Description = "Invest in infrastructure",
                    EffectGdp = 2,
                    EffectStability = 1,
                    Cost = country.Gdp * 0.02
                });
                options.Add(new DecisionOption
                {
                    Id = "austerity",
                    Description = "Implement austerity measures",
                    EffectGdp = -1,
                    EffectStability = -2,
                    Cost = 0
                });
                break;
                
            case DecisionType.Military:
                options.Add(new DecisionOption
                {
                    Id = "mil_buildup",
                    Description = "Military buildup",
                    EffectMilitary = 15,
                    EffectGdp = -1,
                    Cost = country.Gdp * 0.03
                });
                options.Add(new DecisionOption
                {
                    Id = "demilitarize",
                    Description = "Reduce military spending",
                    EffectMilitary = -10,
                    EffectGdp = 2,
                    Cost = 0
                });
                break;
                
            case DecisionType.Diplomatic:
                var potentialAllies = _simulation.Countries.Where(c => c.Id != country.Id && !country.Allies.Contains(c.Id) && !country.Enemies.Contains(c.Id)).ToList();
                if (potentialAllies.Count > 0)
                {
                    options.Add(new DecisionOption
                    {
                        Id = "ally",
                        Description = $"Seek alliance with {potentialAllies[0].Name}",
                        EffectStability = 3,
                        Cost = country.Gdp * 0.01
                    });
                }
                break;
                
            case DecisionType.Domestic:
                options.Add(new DecisionOption
                {
                    Id = "reform",
                    Description = "Implement political reforms",
                    EffectStability = 5,
                    EffectHappiness = 3,
                    Cost = country.Gdp * 0.015
                });
                options.Add(new DecisionOption
                {
                    Id = "crackdown",
                    Description = "Crackdown on dissent",
                    EffectStability = -3,
                    EffectHappiness = -5,
                    Cost = 0
                });
                break;
        }
        
        return options;
    }

    public void SimulateAiDecisions()
    {
        foreach (var country in _simulation.Countries.Where(c => c.Id != _simulation.PlayerCountryId))
        {
            if (_random.NextDouble() < 0.15 * _settings.AiAggressiveness)
            {
                GenerateAiDecision(country);
            }
        }
    }

    private void GenerateAiDecision(Country country)
    {
        var decisionTypes = Enum.GetValues<DecisionType>();
        var type = decisionTypes[_random.Next(decisionTypes.Length)];
        
        var options = GenerateDecisionOptions(country, type);
        
        if (options.Count == 0) return;
        
        var selectedOption = options[_random.Next(options.Count)];
        
        ApplyDecision(country, selectedOption);
    }

    public void ApplyDecision(Country country, DecisionOption option)
    {
        country.Gdp *= (1 + option.EffectGdp / 100);
        country.Stability = Math.Clamp(country.Stability + option.EffectStability, 0, 100);
        country.MilitaryPower = (int)(country.MilitaryPower * (1 + option.EffectMilitary / 100));
        country.Happiness = Math.Clamp(country.Happiness + option.EffectHappiness, 0, 100);
    }
}
