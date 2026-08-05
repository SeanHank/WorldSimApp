using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;
using DiplomaticAction = WorldSimApp.Models.DiplomaticAction;
using PendingPolicy = WorldSimApp.Models.PendingPolicy;

namespace WorldSimApp.Simulation;

public class DiplomaticSystem
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;
    private List<InternationalOrganization> _internationalOrganizations = new();

    public DiplomaticSystem(WorldSimulation simulation)
    {
        _simulation = simulation;
        _random = new Random();
        InitializeInternationalOrganizations();
    }

    private void InitializeInternationalOrganizations()
    {
        _internationalOrganizations = new List<InternationalOrganization>
        {
            new InternationalOrganization
            {
                Id = "UN",
                Name = "Global Alliance Council",
                Type = "Global",
                MemberIds = new List<string>(),
                SecurityCouncilIds = new List<string> { "USA", "CHN", "RUS", "GBR", "FRA" },
                Actions = new List<string> { "peacekeeping", "sanctions", "resolution" }
            },
            new InternationalOrganization
            {
                Id = "NATO",
                Name = "Atlantic Defense Pact",
                Type = "Military",
                MemberIds = new List<string>(),
                DefensePact = true,
                Actions = new List<string> { "collective_defense", "military_exercise" }
            },
            new InternationalOrganization
            {
                Id = "EU",
                Name = "Continental Union",
                Type = "Economic",
                MemberIds = new List<string>(),
                Actions = new List<string> { "trade", "economic_cooperation", "free_movement" }
            },
            new InternationalOrganization
            {
                Id = "OPEC",
                Name = "Energy Cartel",
                Type = "Economic",
                MemberIds = new List<string>(),
                Actions = new List<string> { "oil_production", "price_regulation" }
            },
            new InternationalOrganization
            {
                Id = "WTO",
                Name = "World Trade Organization",
                Type = "Trade",
                MemberIds = new List<string>(),
                Actions = new List<string> { "trade_disputes", "tariff_negotiation" }
            }
        };
    }

    public void SimulateDiplomaticRelations()
    {
        foreach (var country in _simulation.Countries)
        {
            foreach (var otherCountry in _simulation.Countries.Where(c => c.Id != country.Id))
            {
                UpdateDiplomaticRelation(country, otherCountry);
                CheckSanctions(country, otherCountry);
                ProcessTreaties(country, otherCountry);
                EvaluateInternationalObligations(country, otherCountry);
            }
        }
        
        SimulateInternationalOrganizations();
    }

    private void UpdateDiplomaticRelation(Country country, Country otherCountry)
    {
        if (!country.DiplomaticRelations.ContainsKey(otherCountry.Id))
            country.DiplomaticRelations[otherCountry.Id] = 0;

        int currentRelation = country.DiplomaticRelations[otherCountry.Id];

        if (country.Allies.Contains(otherCountry.Id))
            currentRelation += 5;
        else if (country.Enemies.Contains(otherCountry.Id))
            currentRelation -= 10;

        if (country.MemoryOfConflicts.ContainsKey(otherCountry.Id))
        {
            double historicalWeight = CalculateHistoricalWeight(country, otherCountry);
            currentRelation -= (int)(country.MemoryOfConflicts[otherCountry.Id] * historicalWeight);
        }
        
        if (country.Memory.PastConflicts.TryGetValue(otherCountry.Id, out var memoryConflicts))
        {
            currentRelation -= memoryConflicts * 2;
            if (memoryConflicts > 3)
                currentRelation -= 10;
        }
        
        if (country.Memory.Grudges.TryGetValue(otherCountry.Id, out var grudge))
        {
            currentRelation -= (int)(grudge * 5);
        }
        
        if (country.Memory.Favors.TryGetValue(otherCountry.Id, out var favor))
        {
            currentRelation += (int)(favor * 5);
        }

        if (country.CultureGroup == otherCountry.CultureGroup)
            currentRelation += 2;

        if (country.DominantReligion == otherCountry.DominantReligion)
            currentRelation += 1;

        if (HasTradeAgreement(country, otherCountry))
            currentRelation += 3;

        if (country.SanctionedBy.Any(s => s.ImposingCountryId == otherCountry.Id && s.IsActive))
            currentRelation -= 20;
            
        double credibilityImpact = CalculateCredibilityImpact(country, otherCountry);
        currentRelation += (int)credibilityImpact;

        currentRelation = Math.Clamp(currentRelation, -100, 100);
        country.DiplomaticRelations[otherCountry.Id] = currentRelation;
        
        if (!country.DiplomaticHistory.ContainsKey(otherCountry.Id))
            country.DiplomaticHistory[otherCountry.Id] = new List<WorldSimApp.Models.DiplomaticAction>();
        
        country.DiplomaticHistory[otherCountry.Id].Add(new WorldSimApp.Models.DiplomaticAction
        {
            Turn = _simulation.CurrentTurn,
            Type = "relation_update",
            Value = currentRelation
        });
        
        if (country.DiplomaticHistory[otherCountry.Id].Count > 20)
            country.DiplomaticHistory[otherCountry.Id].RemoveAt(0);
    }

    private double CalculateHistoricalWeight(Country country, Country otherCountry)
    {
        double weight = 1.0;
        
        if (country.MemoryOfConflicts.TryGetValue(otherCountry.Id, out var memoryValue) && memoryValue > 50)
        {
            weight *= 1.5;
        }
        
        if (country.Memory.PastConflicts.TryGetValue(otherCountry.Id, out var pastConflicts))
        {
            weight *= (1 + pastConflicts * 0.1);
        }
        
        if (country.LastWarTurn > 0 && _simulation.CurrentTurn - country.LastWarTurn < 20)
        {
            weight *= 1.3;
        }
        
        double trustBonus = country.DiplomaticCredibility / 100;
        weight *= (1 - trustBonus * 0.3);
        
        return weight;
    }

    private double CalculateCredibilityImpact(Country country, Country otherCountry)
    {
        double credibility = country.DiplomaticCredibility;
        
        double trustFactor = (credibility - 50) / 50;
        
        if (country.PastTreatiesFulfilled.ContainsKey(otherCountry.Id))
        {
            int fulfilled = country.PastTreatiesFulfilled[otherCountry.Id];
            int broken = country.PastTreatiesBroken.GetValueOrDefault(otherCountry.Id, 0);
            
            if (fulfilled > broken)
                trustFactor += 0.2;
            else if (broken > fulfilled)
                trustFactor -= 0.3;
        }
        
        return trustFactor * 5;
    }

    private bool HasTradeAgreement(Country country, Country otherCountry)
    {
        return country.TradeAgreements.Any(t => t.PartnerId == otherCountry.Id && t.IsActive);
    }

    private void CheckSanctions(Country country, Country otherCountry)
    {
        if (country.DiplomaticRelations[otherCountry.Id] < -50 && _random.NextDouble() < 0.05)
        {
            if (!country.ActiveSanctions.Any(s => s.TargetCountryId == otherCountry.Id))
            {
                var sanctionType = SelectSanctionType(country, otherCountry);
                var sanction = new Sanction
                {
                    Id = Guid.NewGuid().ToString(),
                    ImposingCountryId = country.Id,
                    TargetCountryId = otherCountry.Id,
                    Type = sanctionType,
                    TurnImplemented = _simulation.CurrentTurn,
                    Duration = 10 + _random.Next(10),
                    RemainingTurns = 10 + _random.Next(10),
                    Reason = GenerateSanctionReason(sanctionType),
                    EconomicImpact = CalculateSanctionImpact(sanctionType, otherCountry),
                    IsActive = true
                };
                
                country.ActiveSanctions.Add(sanction);
                otherCountry.SanctionedBy.Add(sanction);
                
                CheckInternationalOrganizationResponse(country, otherCountry);

                _simulation.Events.Add(new SimulationEvent
                {
                    Turn = _simulation.CurrentTurn,
                    CountryId = country.Id,
                    CountryName = country.Name,
                    Type = "Diplomatic",
                    Title = $"{sanctionType} Implemented",
                    Description = $"{country.Name} has imposed {sanctionType} on {otherCountry.Name}. {sanction.Reason}",
                    ImpactGdp = -2 - sanction.EconomicImpact
                });
            }
        }
        
        foreach (var sanction in country.ActiveSanctions.ToList())
        {
            sanction.RemainingTurns--;
            
            if (sanction.RemainingTurns <= 0 || _random.NextDouble() < 0.1)
            {
                var target = _simulation.Countries.FirstOrDefault(c => c.Id == sanction.TargetCountryId);
                if (target != null && country.DiplomaticRelations[target.Id] > -30)
                {
                    country.ActiveSanctions.Remove(sanction);
                    target.SanctionedBy.Remove(sanction);
                    
                    _simulation.Events.Add(new SimulationEvent
                    {
                        Turn = _simulation.CurrentTurn,
                        CountryId = country.Id,
                        CountryName = country.Name,
                        Type = "Diplomatic",
                        Title = "Sanctions Lifted",
                        Description = $"{country.Name} has lifted {sanction.Type} on {target.Name}.",
                        ImpactGdp = 1 + sanction.EconomicImpact * 0.5
                    });
                }
            }
        }
    }

    private SanctionType SelectSanctionType(Country imposer, Country target)
    {
        var relations = imposer.DiplomaticRelations.GetValueOrDefault(target.Id, 0);
        
        if (relations < -80)
            return _random.NextDouble() < 0.5 ? SanctionType.TradeEmbargo : SanctionType.FinancialSanctions;
        else if (relations < -60)
            return SanctionType.TradeEmbargo;
        else if (relations < -50)
            return _random.NextDouble() < 0.5 ? SanctionType.ArmsEmbargo : SanctionType.TravelBan;
        
        return SanctionType.DiplomaticSanctions;
    }

    private string GenerateSanctionReason(SanctionType type)
    {
        return type switch
        {
            SanctionType.TradeEmbargo => "Violation of trade regulations",
            SanctionType.ArmsEmbargo => "Proliferation concerns",
            SanctionType.FinancialSanctions => "Currency manipulation allegations",
            SanctionType.TravelBan => "Human rights violations",
            SanctionType.DiplomaticSanctions => "Diplomatic misconduct",
            _ => "Policy disagreements"
        };
    }

    private double CalculateSanctionImpact(SanctionType type, Country target)
    {
        double baseImpact = type switch
        {
            SanctionType.TradeEmbargo => 3.0,
            SanctionType.ArmsEmbargo => 1.5,
            SanctionType.FinancialSanctions => 4.0,
            SanctionType.TravelBan => 0.5,
            SanctionType.DiplomaticSanctions => 0.3,
            _ => 1.0
        };
        
        double targetSize = Math.Log10(target.Gdp + 1) / 10;
        return baseImpact * targetSize;
    }

    private void CheckInternationalOrganizationResponse(Country source, Country target)
    {
        foreach (var org in _internationalOrganizations)
        {
            bool sourceMember = org.MemberIds.Contains(source.Id);
            bool targetMember = org.MemberIds.Contains(target.Id);
            
            if (!sourceMember || !targetMember) continue;
            
            if (org.Type == "Global" && org.SecurityCouncilIds.Contains(source.Id))
            {
                if (_random.NextDouble() < 0.3)
                {
                    _simulation.Events.Add(new SimulationEvent
                    {
                        Turn = _simulation.CurrentTurn,
                        CountryId = "WORLD",
                        CountryName = org.Name,
                        Type = "Diplomatic",
                        Title = $"{org.Name} Response",
                        Description = $"{org.Name} Security Council debates sanctions between {source.Name} and {target.Name}.",
                        ImpactStability = -1
                    });
                }
            }
            
            if (org.Type == "Military" && org.DefensePact)
            {
                if (source.Allies.Any(a => org.MemberIds.Contains(a)))
                {
                    double collectiveAction = _random.NextDouble() * 20;
                    target.Stability -= collectiveAction;
                }
            }
        }
    }

    private void ProcessTreaties(Country country, Country otherCountry)
    {
        foreach (var treaty in country.Treaties.Where(t => t.IsActive && t.Signatories.Contains(otherCountry.Id)))
        {
            bool treatyRespected = _random.NextDouble() < country.DiplomaticCredibility / 100;
            
            if (treatyRespected)
            {
                if (treaty.Effects.ContainsKey("trade_bonus"))
                {
                    country.EconomicGrowth += treaty.Effects["trade_bonus"];
                }
                if (treaty.Effects.ContainsKey("defense"))
                {
                    country.MilitaryPower = (int)(country.MilitaryPower * 1.05);
                }
                
                if (!country.PastTreatiesFulfilled.ContainsKey(otherCountry.Id))
                    country.PastTreatiesFulfilled[otherCountry.Id] = 0;
                country.PastTreatiesFulfilled[otherCountry.Id]++;
            }
            else
            {
                if (!country.PastTreatiesBroken.ContainsKey(otherCountry.Id))
                    country.PastTreatiesBroken[otherCountry.Id] = 0;
                country.PastTreatiesBroken[otherCountry.Id]++;
                
                country.DiplomaticCredibility = Math.Max(0, country.DiplomaticCredibility - 2);
                
                _simulation.Events.Add(new SimulationEvent
                {
                    Turn = _simulation.CurrentTurn,
                    CountryId = country.Id,
                    CountryName = country.Name,
                    Type = "Diplomatic",
                    Title = "Treaty Violation",
                    Description = $"{country.Name} has violated the {treaty.Name} treaty with {otherCountry.Name}.",
                    ImpactStability = -3,
                    ImpactHappiness = -2
                });
            }
            
            int turnSigned = treaty.TurnSigned;
            if (_simulation.CurrentTurn - turnSigned > treaty.Duration && treaty.Duration > 0)
            {
                treaty.IsActive = false;
            }
        }
    }

    private void EvaluateInternationalObligations(Country country, Country otherCountry)
    {
        foreach (var org in _internationalOrganizations)
        {
            if (!org.MemberIds.Contains(country.Id)) continue;
            
            if (org.Type == "Military" && org.DefensePact)
            {
                if (country.Allies.Contains(otherCountry.Id) && !org.MemberIds.Contains(otherCountry.Id))
                {
                    if (_random.NextDouble() < 0.05)
                    {
                        double membershipPressure = _random.NextDouble() * 10;
                        country.GovernmentApproval -= membershipPressure * 0.1;
                        
                        _simulation.Events.Add(new SimulationEvent
                        {
                            Turn = _simulation.CurrentTurn,
                            CountryId = country.Id,
                            CountryName = country.Name,
                            Type = "Diplomatic",
                            Title = $"{org.Name} Expansion Debate",
                            Description = $"{org.Name} members discuss potential membership for {otherCountry.Name}.",
                            ImpactStability = -1
                        });
                    }
                }
            }
            
            if (org.Type == "Economic")
            {
                double economicCooperation = country.DiplomaticRelations[otherCountry.Id] / 100.0;
                if (economicCooperation > 0.5 && _random.NextDouble() < 0.02)
                {
                    country.EconomicGrowth += 0.2;
                }
            }
        }
    }

    private void SimulateInternationalOrganizations()
    {
        foreach (var org in _internationalOrganizations)
        {
            if (org.Id == "UN")
            {
                if (_random.NextDouble() < 0.03)
                {
                    var conflict = _simulation.Wars.FirstOrDefault(w => w.Status == WarStatus.War);
                    if (conflict != null)
                    {
                        _simulation.Events.Add(new SimulationEvent
                        {
                            Turn = _simulation.CurrentTurn,
                            CountryId = "WORLD",
                            CountryName = "Global Alliance Council",
                            Type = "Diplomatic",
                            Title = "UN Peacekeeping Mission",
                            Description = "The Global Alliance Council authorizes a peacekeeping mission to address ongoing conflict.",
                            ImpactStability = 2
                        });
                    }
                }
            }
            
            if (org.Id == "NATO")
            {
                var natoMembers = _simulation.Countries.Where(c => org.MemberIds.Contains(c.Id)).ToList();
                foreach (var member in natoMembers)
                {
                    if (member.WarFatigue > 50)
                    {
                        member.MilitarySpendingPercent *= 0.95;
                    }
                }
            }
            
            if (org.Id == "OPEC")
            {
                var oilProducers = _simulation.Countries.Where(c => 
                    c.StrategicResources.ContainsKey("Oil") && org.MemberIds.Contains(c.Id)).ToList();
                
                if (oilProducers.Count > 0 && _random.NextDouble() < 0.1)
                {
                    double priceEffect = (_random.NextDouble() - 0.5) * 0.2;
                    foreach (var producer in oilProducers)
                    {
                        producer.EconomicGrowth += priceEffect;
                    }
                }
            }
        }
    }
}
