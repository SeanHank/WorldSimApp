using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;

namespace WorldSimApp.Simulation;

public class PoliticalSystem
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;
    private Dictionary<string, PolicyImplementation> _activePolicies = new();

    public PoliticalSystem(WorldSimulation simulation)
    {
        _simulation = simulation;
        _random = new Random();
    }

    public void SimulatePoliticalSystem()
    {
        foreach (var country in _simulation.Countries)
        {
            SimulatePolicyShifts(country);
            CheckElections(country);
            UpdatePublicOpinion(country);
            ImplementPoliciesWithLag(country);
            UpdateGovernmentApproval(country);
            UpdatePartyPolicies(country);
            HandlePoliticalCrises(country);
        }
    }
    
    private void SimulatePolicyShifts(Country country)
    {
        double shiftChance = 0.08 * _simulation.Settings.RandomnessMultiplier;
        
        if (RandomManager.Chance(shiftChance))
        {
            var policyShifts = new Dictionary<string, Action>
            {
                ["economic_reform"] = () => {
                    country.EconomicGrowth += RandomManager.NextRange(0.5, 2.0);
                    country.GovernmentSpending += RandomManager.NextRange(-2, 3);
                },
                ["military_buildup"] = () => {
                    country.MilitarySpending += RandomManager.NextRange(0.5, 1.5);
                    country.Gdp -= country.Gdp * 0.01;
                },
                ["austerity"] = () => {
                    country.GovernmentSpending -= RandomManager.NextRange(1, 3);
                    country.Happiness -= RandomManager.NextRange(2, 5);
                },
                ["populist_spending"] = () => {
                    country.GovernmentSpending += RandomManager.NextRange(2, 5);
                    country.Inflation += RandomManager.NextRange(0.5, 2);
                },
                ["education_reform"] = () => {
                    country.EducationLevel += RandomManager.NextRange(0.5, 1.5);
                    country.Happiness += RandomManager.NextRange(1, 3);
                },
                ["infrastructure_boom"] = () => {
                    country.InfrastructureQuality += RandomManager.NextRange(1, 3);
                    country.Gdp += country.Gdp * 0.005;
                }
            };
            
            var shiftKey = RandomManager.WeightedRandom(policyShifts.ToDictionary(k => k.Key, v => 1.0));
            policyShifts[shiftKey].Invoke();
            
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = country.Id,
                CountryName = country.Name,
                Type = "Political",
                Title = $"{country.RulingParty} Announces {shiftKey.Replace("_", " ")}",
                Description = $"{country.Name} has implemented new policy: {shiftKey.Replace("_", " ")}",
                ImpactStability = RandomManager.NextRange(-3, 2)
            });
        }
    }

    private void CheckElections(Country country)
    {
        if (country.CurrentElectionTurn + country.ElectionCycleYears <= _simulation.CurrentTurn)
        {
            HoldElection(country);
        }
    }

    private void HoldElection(Country country)
    {
        var parties = GetPartiesForCountry(country);
        double totalVotes = 0;
        var results = new Dictionary<string, double>();

        foreach (var party in parties)
        {
            double baseVote = party.BaseSupport;
            double economyBonus = country.EconomicGrowth > 3 ? 5 : country.EconomicGrowth < 0 ? -5 : 0;
            double stabilityBonus = country.Stability > 70 ? 3 : country.Stability < 40 ? -5 : 0;
            double scandalPenalty = country.ScandalLevel > 0 ? -country.ScandalLevel : 0;
            
            double vote = Math.Clamp(baseVote + economyBonus + stabilityBonus + scandalPenalty + (_random.NextDouble() - 0.5) * 10, 0, 60);
            results[party.Id] = vote;
            totalVotes += vote;
        }

        foreach (var key in results.Keys.ToList())
        {
            results[key] = results[key] / totalVotes * 100;
        }

        var winner = results.OrderByDescending(x => x.Value).First();
        
        country.PreviousRulingParty = country.RulingParty;
        country.RulingParty = winner.Key;
        country.PoliticalSpectrum = GetPartySpectrum(winner.Key);
        
        double transitionCost = CalculateTransitionCost(country, country.PreviousRulingParty, country.RulingParty);
        country.Stability -= transitionCost;
        
        country.GovernmentApproval = winner.Value + (_random.NextDouble() - 0.5) * 10;
        country.GovernmentApproval = Math.Clamp(country.GovernmentApproval, 0, 100);
        
        country.CurrentElectionTurn = _simulation.CurrentTurn;
        country.NextElectionTurn = _simulation.CurrentTurn + country.ElectionCycleYears;
        
        country.ScandalLevel = 0;

        var election = new Election
        {
            Id = Guid.NewGuid().ToString(),
            CountryId = country.Id,
            Turn = _simulation.CurrentTurn,
            IsHeld = true,
            WinnerId = winner.Key,
            WinnerName = winner.Key,
            VoterTurnout = 60 + _random.NextDouble() * 20,
            VoteResults = results,
            KeyIssue = DetermineKeyIssue(country),
            WinnerAgenda = string.Join(", ", GetPartyAgenda(winner.Key))
        };
        country.ElectionHistory.Add(election);

        _simulation.Events.Add(new SimulationEvent
        {
            Turn = _simulation.CurrentTurn,
            CountryId = country.Id,
            CountryName = country.Name,
            Type = "Political",
            Title = $"Election Held: {winner.Key} Wins",
            Description = $"National elections in {country.Name} have concluded. {winner.Key} party has won with {winner.Value:F1}% of the vote.",
            ImpactStability = _random.NextDouble() * 5 - 2
        });
    }

    private string DetermineKeyIssue(Country country)
    {
        var issues = new[] { "Economy", "Healthcare", "Education", "Defense", "Immigration", "Environment" };
        return issues[_random.Next(issues.Length)];
    }

    private List<string> GetPartyAgenda(string partyId)
    {
        return partyId switch
        {
            "Communist" or "Socialist" => new List<string> { "Welfare", "Nationalization" },
            "SocialDemocrat" => new List<string> { "Healthcare", "Education" },
            "Liberal" => new List<string> { "FreeTrade", "Environment" },
            "Conservative" => new List<string> { "Military", "LawOrder" },
            "Nationalist" => new List<string> { "Immigration", "Sovereignty" },
            _ => new List<string> { "Moderate" }
        };
    }

    private double CalculateTransitionCost(Country country, string previousParty, string currentParty)
    {
        if (string.IsNullOrEmpty(previousParty) || previousParty == currentParty)
            return 0;
        
        double ideologicalDistance = Math.Abs(GetPartyPosition(currentParty) - GetPartyPosition(previousParty));
        double policyReversalCost = ideologicalDistance * 2;
        
        double institutionalStrength = country.Stability / 100;
        double netCost = policyReversalCost * (1 - institutionalStrength * 0.5);
        
        return netCost;
    }

    private int GetPartyPosition(string partyId)
    {
        return partyId switch
        {
            "Communist" => 1,
            "Socialist" => 2,
            "SocialDemocrat" => 3,
            "Centrist" => 4,
            "Liberal" => 5,
            "Conservative" => 6,
            "Nationalist" => 7,
            _ => 4
        };
    }

    private void UpdatePublicOpinion(Country country)
    {
        double economicImpact = (country.EconomicGrowth - 2) * 1.5;
        double inflationImpact = -(country.Inflation - 3) * 0.5;
        double unemploymentImpact = -(country.Unemployment - 5) * 0.8;
        double mediaInfluence = _random.NextDouble() * 2 - 1;
        
        country.PublicOpinion += economicImpact + inflationImpact + unemploymentImpact + mediaInfluence;
        country.PublicOpinion = Math.Clamp(country.PublicOpinion, 0, 100);
        
        if (_random.NextDouble() < 0.1)
        {
            country.IssueSalience["Economy"] = Math.Clamp(country.IssueSalience.GetValueOrDefault("Economy", 30) + _random.NextDouble() * 10 - 5, 10, 50);
            country.IssueSalience["Security"] = Math.Clamp(country.IssueSalience.GetValueOrDefault("Security", 20) + _random.NextDouble() * 5 - 2.5, 5, 40);
            country.IssueSalience["Environment"] = Math.Clamp(country.IssueSalience.GetValueOrDefault("Environment", 15) + _random.NextDouble() * 3, 5, 35);
        }
        
        country.ApprovalTrend.Add(country.GovernmentApproval);
        if (country.ApprovalTrend.Count > 5)
            country.ApprovalTrend.RemoveAt(0);
    }

    private void ImplementPoliciesWithLag(Country country)
    {
        var pendingPolicies = _activePolicies.Where(p => p.Value.CountryId == country.Id).ToList();
        
        foreach (var policy in pendingPolicies)
        {
            if (policy.Value.RemainingTurns > 0)
            {
                policy.Value.RemainingTurns--;
                ApplyPolicyEffect(country, policy.Value, isImplementationPhase: true);
            }
            else
            {
                ApplyPolicyEffect(country, policy.Value, isImplementationPhase: false);
                _activePolicies.Remove(policy.Key);
            }
        }
        
        if (country.NewPoliciesToImplement.Count > 0)
        {
            foreach (var newPolicy in country.NewPoliciesToImplement)
            {
                var implementation = new PolicyImplementation
                {
                    PolicyId = newPolicy.Key,
                    CountryId = country.Id,
                    RemainingTurns = newPolicy.Value.ImplementationDelay,
                    TotalEffect = newPolicy.Value.EffectGdp
                };
                _activePolicies[$"{country.Id}_{newPolicy.Key}"] = implementation;
            }
            country.NewPoliciesToImplement.Clear();
        }
    }

    private void ApplyPolicyEffect(Country country, PolicyImplementation implementation, bool isImplementationPhase)
    {
        double effectivenessMultiplier = isImplementationPhase ? 0.3 : 0.7;
        
        if (implementation.TotalEffect != 0)
        {
            country.EconomicGrowth += implementation.TotalEffect * effectivenessMultiplier * 0.1;
        }
    }

    private void UpdateGovernmentApproval(Country country)
    {
        double trendEffect = 0;
        if (country.ApprovalTrend.Count >= 2)
        {
            trendEffect = country.ApprovalTrend[^1] - country.ApprovalTrend[0];
        }
        
        double growthImpact = country.EconomicGrowth > 3 ? 2 : country.EconomicGrowth < -2 ? -3 : 0;
        double stabilityImpact = country.Stability > 70 ? 1 : country.Stability < 40 ? -2 : 0;
        double happinessImpact = (country.Happiness - 50) / 25;
        double scandalImpact = -country.ScandalLevel * 0.5;
        
        country.GovernmentApproval += growthImpact + stabilityImpact + happinessImpact + scandalImpact + trendEffect * 0.2 + (_random.NextDouble() - 0.5);
        country.GovernmentApproval = Math.Clamp(country.GovernmentApproval, 0, 100);

        if (country.GovernmentApproval < 30 && _random.NextDouble() < 0.1)
        {
            country.PoliticalSpectrum = country.PoliticalSpectrum == "Left" ? "Right" : "Left";
            country.GovernmentApproval += 10;
        }
        
        if (country.GovernmentApproval > 80 && _random.NextDouble() < 0.05)
        {
            country.ScandalLevel = Math.Min(20, country.ScandalLevel + _random.NextDouble() * 5);
        }
    }

    private void UpdatePartyPolicies(Country country)
    {
        if (country.PoliticalSpectrum == "Left")
        {
            country.PolicyAgenda = new List<string> { "Welfare", "Nationalization", "HighTax" };
            country.EconomicGrowth *= 0.98;
            country.Happiness = Math.Clamp(country.Happiness + 1, 0, 100);
        }
        else if (country.PoliticalSpectrum == "Right")
        {
            country.PolicyAgenda = new List<string> { "FreeMarket", "Military", "LowTax" };
            country.EconomicGrowth *= 1.02;
            country.Stability = Math.Clamp(country.Stability + 0.5, 0, 100);
        }
        
        if (country.IssueSalience.TryGetValue("Economy", out var economySalience) && economySalience > 35)
        {
            if (!country.PolicyAgenda.Contains("EconomicReform"))
            {
                country.NewPoliciesToImplement["EconomicReform"] = new WorldSimApp.Models.PendingPolicy
                {
                    EffectGdp = 1.5,
                    ImplementationDelay = 2
                };
            }
        }
    }

    private void HandlePoliticalCrises(Country country)
    {
        if (country.ScandalLevel > 15 && _random.NextDouble() < country.ScandalLevel / 100)
        {
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = country.Id,
                CountryName = country.Name,
                Type = "Political",
                Title = "Political Crisis",
                Description = $"A major scandal has rocked {country.Name}'s government, causing a political crisis.",
                ImpactStability = -5,
                ImpactHappiness = -5
            });
            
            country.ScandalLevel = Math.Max(0, country.ScandalLevel - 10);
            country.GovernmentApproval -= 10;
        }
        
        if (country.Stability < 30 && _random.NextDouble() < 0.05)
        {
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = country.Id,
                CountryName = country.Name,
                Type = "Political",
                Title = "Constitutional Crisis",
                Description = $"{country.Name} is facing a constitutional crisis as institutions struggle.",
                ImpactStability = -10
            });
            
            country.GovernmentApproval -= 15;
        }
    }

    private List<Party> GetPartiesForCountry(Country country)
    {
        var parties = new List<Party>();
        
        parties.Add(new Party
        {
            Id = "Communist",
            Name = "Communist Party",
            Ideology = PartyIdeology.FarLeft,
            EconomicPolicy = "Planned",
            SocialPolicy = "Collectivist",
            ForeignPolicy = "AntiWestern",
            BaseSupport = 8,
            Populism = 0.4
        });
        
        parties.Add(new Party
        {
            Id = "Socialist",
            Name = "Socialist Party",
            Ideology = PartyIdeology.Left,
            EconomicPolicy = "Socialist",
            SocialPolicy = "Progressive",
            ForeignPolicy = "Neutral",
            BaseSupport = 12,
            Populism = 0.35
        });
        
        parties.Add(new Party
        {
            Id = "SocialDemocrat",
            Name = "Social Democratic Party",
            Ideology = PartyIdeology.CenterLeft,
            EconomicPolicy = "Mixed",
            SocialPolicy = "Progressive",
            ForeignPolicy = "Cooperative",
            BaseSupport = 18,
            Populism = 0.25
        });
        
        parties.Add(new Party
        {
            Id = "Liberal",
            Name = "Liberal Party",
            Ideology = PartyIdeology.Center,
            EconomicPolicy = "Market",
            SocialPolicy = "Liberal",
            ForeignPolicy = "Internationalist",
            BaseSupport = 20,
            Populism = 0.2
        });
        
        parties.Add(new Party
        {
            Id = "Centrist",
            Name = "Centrist Party",
            Ideology = PartyIdeology.Center,
            EconomicPolicy = "Mixed",
            SocialPolicy = "Moderate",
            ForeignPolicy = "Neutral",
            BaseSupport = 22,
            Populism = 0.3
        });
        
        parties.Add(new Party
        {
            Id = "Conservative",
            Name = "Conservative Party",
            Ideology = PartyIdeology.Right,
            EconomicPolicy = "Market",
            SocialPolicy = "Traditional",
            ForeignPolicy = "Nationalist",
            BaseSupport = 18,
            Populism = 0.25
        });
        
        parties.Add(new Party
        {
            Id = "Nationalist",
            Name = "Nationalist Party",
            Ideology = PartyIdeology.FarRight,
            EconomicPolicy = "Nationalist",
            SocialPolicy = "Traditional",
            ForeignPolicy = "Aggressive",
            BaseSupport = 10,
            Populism = 0.5
        });
        
        foreach (var party in parties)
        {
            party.BaseSupport += RandomManager.NextRange(-3, 3);
            party.BaseSupport = Math.Clamp(party.BaseSupport, 5, 35);
            
            party.PolicyPositions["Economic"] = GetPolicyPosition(party.Ideology, "Economic");
            party.PolicyPositions["Social"] = GetPolicyPosition(party.Ideology, "Social");
            party.PolicyPositions["Foreign"] = GetPolicyPosition(party.Ideology, "Foreign");
        }
        
        return parties.OrderByDescending(p => p.BaseSupport).ToList();
    }
    
    private double GetPolicyPosition(PartyIdeology ideology, string category)
    {
        double position = ideology switch
        {
            PartyIdeology.FarLeft => -3,
            PartyIdeology.Left => -2,
            PartyIdeology.CenterLeft => -1,
            PartyIdeology.Center => 0,
            PartyIdeology.CenterRight => 1,
            PartyIdeology.Right => 2,
            PartyIdeology.FarRight => 3,
            _ => 0
        };
        
        if (category == "Social" || category == "Foreign")
            position *= 0.8;
            
        return position;
    }

    private string GetPartySpectrum(string partyId)
    {
        return partyId switch
        {
            "Communist" or "Socialist" => "Left",
            "SocialDemocrat" or "Liberal" => "CenterLeft",
            "Centrist" => "Center",
            "Conservative" or "Nationalist" => "Right",
            _ => "Center"
        };
    }
}

public class PolicyImplementation
{
    public string PolicyId { get; set; } = string.Empty;
    public string CountryId { get; set; } = string.Empty;
    public int RemainingTurns { get; set; }
    public double TotalEffect { get; set; }
}
