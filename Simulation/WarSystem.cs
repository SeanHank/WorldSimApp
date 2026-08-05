using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;

namespace WorldSimApp.Simulation;

public class EnhancedWarSimulator
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;
    private readonly GameSettings _settings;
    private Dictionary<string, WarTheater> _theaters = new();

    public EnhancedWarSimulator(WorldSimulation simulation, GameSettings settings)
    {
        _simulation = simulation;
        _settings = settings;
        _random = new Random();
        InitializeTerrainData();
    }

    private void InitializeTerrainData()
    {
        _theaters = new Dictionary<string, WarTheater>
        {
            ["Middle East"] = new WarTheater { Region = "Middle East", TerrainType = "Desert", DefenseBonus = 1.2, SupplyDifficulty = 1.5, AirSuperiorityFactor = 1.3 },
            ["Asia"] = new WarTheater { Region = "Asia", TerrainType = "Mountainous", DefenseBonus = 1.3, SupplyDifficulty = 1.4, AirSuperiorityFactor = 1.1 },
            ["Europe"] = new WarTheater { Region = "Europe", TerrainType = "Urban", DefenseBonus = 1.1, SupplyDifficulty = 1.0, AirSuperiorityFactor = 1.0 },
            ["North America"] = new WarTheater { Region = "North America", TerrainType = "Plains", DefenseBonus = 1.0, SupplyDifficulty = 0.8, AirSuperiorityFactor = 1.2 },
            ["South America"] = new WarTheater { Region = "South America", TerrainType = "Jungle", DefenseBonus = 1.25, SupplyDifficulty = 1.6, AirSuperiorityFactor = 0.9 },
            ["Africa"] = new WarTheater { Region = "Africa", TerrainType = "Desert", DefenseBonus = 1.15, SupplyDifficulty = 1.5, AirSuperiorityFactor = 1.0 }
        };
    }

    public void SimulateDiplomaticRelations()
    {
        foreach (var country in _simulation.Countries)
        {
            foreach (var enemyId in country.Enemies.ToList())
            {
                var enemy = _simulation.Countries.FirstOrDefault(c => c.Id == enemyId);
                if (enemy == null) continue;
                
                if (_random.NextDouble() < 0.02 * _settings.AiAggressiveness)
                {
                    double strategicAdvantage = CalculateStrategicAdvantage(country, enemy);
                    if (country.MilitaryPower > enemy.MilitaryPower * 1.3 && strategicAdvantage > 0.5)
                    {
                        StartWar(country, enemy);
                    }
                }
            }
        }
    }

    private double CalculateStrategicAdvantage(Country attacker, Country defender)
    {
        double terrainAdvantage = 1.0;
        if (_theaters.TryGetValue(defender.Region, out var theater))
        {
            terrainAdvantage = attacker.Region == defender.Region ? theater.DefenseBonus : 1.0 / theater.DefenseBonus;
        }

        double allianceFactor = CalculateAllianceFactor(attacker, defender);
        double logisticsFactor = CalculateLogisticsFactor(attacker, defender);
        double motivationFactor = CalculateMotivationFactor(attacker, defender);

        return terrainAdvantage * allianceFactor * logisticsFactor * motivationFactor;
    }

    private double CalculateAllianceFactor(Country attacker, Country defender)
    {
        int attackerAllies = attacker.Allies.Count(a => _simulation.Countries.Any(c => c.Id == a));
        int defenderAllies = defender.Allies.Count(a => _simulation.Countries.Any(c => c.Id == a));
        
        double allianceStrength = (attackerAllies * 0.3) - (defenderAllies * 0.2);
        return Math.Max(0.5, 1.0 + allianceStrength);
    }

    private double CalculateLogisticsFactor(Country attacker, Country defender)
    {
        double distanceFactor = 1.0;
        if (_theaters.TryGetValue(defender.Region, out var theater))
        {
            distanceFactor = attacker.Region == defender.Region ? 1.0 : theater.SupplyDifficulty;
        }

        double resourceFactor = attacker.ResourceDependency.Values.Sum() > 0 ? 0.8 : 1.0;
        double economicFactor = attacker.EconomicGrowth > 0 ? 1.1 : 0.9;

        return distanceFactor * resourceFactor * economicFactor;
    }

    private double CalculateMotivationFactor(Country country, Country enemy)
    {
        double warFatiguePenalty = 1.0 - (country.WarFatigue / 100) * 0.5;
        
        double historicalGrievance = 0;
        if (country.MemoryOfConflicts.TryGetValue(enemy.Id, out var memory))
        {
            historicalGrievance = memory / 100.0;
        }

        double ideologicalFactor = country.PoliticalSpectrum != enemy.PoliticalSpectrum ? 1.1 : 1.0;
        
        return warFatiguePenalty * (1 + historicalGrievance) * ideologicalFactor;
    }

    public void StartWar(Country attacker, Country defender)
    {
        if (_simulation.Wars.Any(w => (w.AttackerId == attacker.Id && w.DefenderId == defender.Id) ||
                          (w.AttackerId == defender.Id && w.DefenderId == attacker.Id)))
            return;
        
        if (!attacker.Memory.PastConflicts.ContainsKey(defender.Id))
            attacker.Memory.PastConflicts[defender.Id] = 0;
        attacker.Memory.PastConflicts[defender.Id]++;
        attacker.Memory.LastWarTurn = _simulation.CurrentTurn;
        
        if (!defender.Memory.PastConflicts.ContainsKey(attacker.Id))
            defender.Memory.PastConflicts[attacker.Id] = 0;
        defender.Memory.PastConflicts[attacker.Id]++;
        defender.Memory.LastWarTurn = _simulation.CurrentTurn;
        
        if (!attacker.Memory.Grudges.ContainsKey(defender.Id))
            attacker.Memory.Grudges[defender.Id] = 0;
        attacker.Memory.Grudges[defender.Id] += 2;
        
        if (!defender.Memory.Grudges.ContainsKey(attacker.Id))
            defender.Memory.Grudges[attacker.Id] = 0;
        defender.Memory.Grudges[attacker.Id] += 2;
        
        attacker.Enemies.Add(defender.Id);
        defender.Enemies.Add(attacker.Id);
        
        var theater = _theaters.GetValueOrDefault(defender.Region, new WarTheater { Region = defender.Region });
        
        var war = new War
        {
            Name = $"{attacker.Name} vs {defender.Name}",
            AttackerId = attacker.Id,
            DefenderId = defender.Id,
            StartTurn = _simulation.CurrentTurn,
            CurrentTurn = _simulation.CurrentTurn,
            Status = WarStatus.War,
            Result = WarResult.Ongoing,
            TerrainType = theater.TerrainType,
            DefenseBonus = theater.DefenseBonus,
            SupplyDifficulty = theater.SupplyDifficulty
        };
        
        _simulation.Wars.Add(war);
        
        attacker.WarsFought++;
        defender.WarsFought++;
        
        _simulation.Events.Add(new SimulationEvent
        {
            Turn = _simulation.CurrentTurn,
            CountryId = attacker.Id,
            CountryName = attacker.Name,
            Type = "Military",
            Title = "War Declared!",
            Description = $"{attacker.Name} has declared war on {defender.Name}! The conflict will take place in {theater.TerrainType} terrain.",
            ImpactGdp = -5,
            ImpactStability = -10,
            ImpactMilitary = 0
        });
    }

    public void SimulateWars()
    {
        var warsToRemove = new List<War>();
        
        foreach (var war in _simulation.Wars.Where(w => w.Status == WarStatus.War).ToList())
        {
            var attacker = _simulation.Countries.FirstOrDefault(c => c.Id == war.AttackerId);
            var defender = _simulation.Countries.FirstOrDefault(c => c.Id == war.DefenderId);
            
            if (attacker == null || defender == null)
            {
                warsToRemove.Add(war);
                continue;
            }
            
            war.CurrentTurn++;
            
            bool allyJoined = CheckAndProcessAllianceInvolvement(war, attacker, defender);
            
            int baseCasualties = 50 + _random.Next(100);
            double terrainMultiplier = war.DefenseBonus;
            double logisticsPenalty = war.SupplyDifficulty;
            
            int casualties = (int)(baseCasualties * terrainMultiplier * logisticsPenalty * _settings.AiAggressiveness);
            
            double attackerPower = attacker.MilitaryPower * GetMoraleMultiplier(attacker);
            double defenderPower = defender.MilitaryPower * GetMoraleMultiplier(defender);
            
            double allianceSupport = CalculateAllianceCombatSupport(attacker, defender);
            attackerPower *= allianceSupport;
            
            double attackerWinChance = attackerPower / (attackerPower + defenderPower);
            
            if (_random.NextDouble() < attackerWinChance)
            {
                war.AttackerDeaths += casualties / 2;
                war.DefenderDeaths += casualties;
                
                int attackerLoss = (int)(casualties * 0.3 * war.SupplyDifficulty);
                attacker.MilitaryPower = Math.Max(1, attacker.MilitaryPower - attackerLoss);
                attacker.Gdp *= 0.985;
                attacker.Stability = Math.Max(0, attacker.Stability - 1.5);
                
                defender.MilitaryPower = Math.Max(1, (int)(defender.MilitaryPower * 0.92));
                defender.Gdp *= 0.975;
                defender.Stability = Math.Max(0, defender.Stability - 2);
                
                war.AttackerAdvances = true;
            }
            else
            {
                war.DefenderDeaths += casualties / 2;
                war.AttackerDeaths += casualties;
                
                int defenderLoss = (int)(casualties * 0.25 * war.DefenseBonus);
                defender.MilitaryPower = Math.Max(1, defender.MilitaryPower - defenderLoss);
                defender.Gdp *= 0.98;
                defender.Stability = Math.Max(0, defender.Stability - 1.5);
                
                attacker.MilitaryPower = Math.Max(1, (int)(attacker.MilitaryPower * 0.93));
                attacker.Gdp *= 0.98;
                attacker.Stability = Math.Max(0, attacker.Stability - 2);
                
                war.AttackerAdvances = false;
            }
            
            UpdateWarFatigue(attacker, defender, war);
            
            if (war.CurrentTurn - war.StartTurn > 10 && _random.NextDouble() > 0.6)
            {
                bool warEnded = TryEndWar(war, attacker, defender);
                if (warEnded)
                {
                    warsToRemove.Add(war);
                }
            }
            
            if (war.CurrentTurn - war.StartTurn > 20)
            {
                war.Status = WarStatus.Ceasefire;
                war.Result = WarResult.Stalemate;
                warsToRemove.Add(war);
                
                _simulation.Events.Add(new SimulationEvent
                {
                    Turn = _simulation.CurrentTurn,
                    CountryId = attacker.Id,
                    CountryName = attacker.Name,
                    Type = "Military",
                    Title = "War Stalemate",
                    Description = $"The war between {attacker.Name} and {defender.Name} has reached a stalemate. Both sides agree to a ceasefire.",
                    ImpactStability = -5
                });
            }
        }
        
        foreach (var w in warsToRemove)
        {
            _simulation.Wars.Remove(w);
        }
    }

    private bool CheckAndProcessAllianceInvolvement(War war, Country attacker, Country defender)
    {
        bool allyJoined = false;
        
        foreach (var allyId in attacker.Allies.ToList())
        {
            var ally = _simulation.Countries.FirstOrDefault(c => c.Id == allyId);
            if (ally == null) continue;
            
            if (ally.DiplomaticRelations.TryGetValue(defender.Id, out var relation) && relation > 30)
            {
                if (_random.NextDouble() < 0.15)
                {
                    war.AlliedForces[allyId] = ally.MilitaryPower / 10;
                    allyJoined = true;
                    
                    _simulation.Events.Add(new SimulationEvent
                    {
                        Turn = _simulation.CurrentTurn,
                        CountryId = ally.Id,
                        CountryName = ally.Name,
                        Type = "Military",
                        Title = "Alliance Military Support",
                        Description = $"{ally.Name} has joined the war in support of {attacker.Name}.",
                        ImpactMilitary = 10
                    });
                }
            }
        }
        
        return allyJoined;
    }

    private double GetMoraleMultiplier(Country country)
    {
        double morale = 1.0;
        morale += country.GovernmentApproval / 200;
        morale -= country.WarFatigue / 200;
        morale += (100 - country.CrimeRate) / 200;
        return Math.Clamp(morale, 0.5, 1.5);
    }

    private double CalculateAllianceCombatSupport(Country attacker, Country defender)
    {
        double support = 1.0;
        
        foreach (var war in _simulation.Wars.Where(w => w.Status == WarStatus.War))
        {
            if (war.AlliedForces.ContainsKey(attacker.Id))
            {
                support += war.AlliedForces[attacker.Id] / attacker.MilitaryPower;
            }
        }
        
        return support;
    }

    private void UpdateWarFatigue(Country attacker, Country defender, War war)
    {
        int duration = war.CurrentTurn - war.StartTurn;
        
        if (duration > 3)
        {
            attacker.WarFatigue = Math.Min(100, attacker.WarFatigue + (duration - 3) * 1.5);
            attacker.Stability = Math.Clamp(attacker.Stability - (duration - 3) * 0.3, 0, 100);
        }
        
        defender.WarFatigue = Math.Min(100, defender.WarFatigue + (duration - 2) * 2);
        defender.Stability = Math.Clamp(defender.Stability - (duration - 2) * 0.4, 0, 100);
        
        if (duration > 8)
        {
            attacker.Happiness = Math.Clamp(attacker.Happiness - (duration - 8) * 0.5, 0, 100);
            defender.Happiness = Math.Clamp(defender.Happiness - (duration - 8) * 0.8, 0, 100);
        }
    }

    private bool TryEndWar(War war, Country attacker, Country defender)
    {
        double attackerStrength = attacker.MilitaryPower * (1 - attacker.WarFatigue / 100);
        double defenderStrength = defender.MilitaryPower * (1 - defender.WarFatigue / 100);
        
        if (attackerStrength > defenderStrength * 1.5)
        {
            war.Status = WarStatus.Ceasefire;
            war.Result = WarResult.AttackerWins;
            war.Reparations = defender.Gdp * 0.08;
            defender.Gdp -= war.Reparations;
            
            attacker.WarsWon++;
            
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = attacker.Id,
                CountryName = attacker.Name,
                Type = "Diplomatic",
                Title = "War Victory",
                Description = $"{attacker.Name} has won the war against {defender.Name}. Victory is assured.",
                ImpactGdp = 5,
                ImpactStability = 10
            });
        }
        else if (defenderStrength > attackerStrength * 1.3)
        {
            war.Status = WarStatus.Ceasefire;
            war.Result = WarResult.DefenderWins;
            war.Reparations = attacker.Gdp * 0.05;
            attacker.Gdp -= war.Reparations;
            
            defender.WarsWon++;
            
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = defender.Id,
                CountryName = defender.Name,
                Type = "Diplomatic",
                Title = "War Defended",
                Description = $"{defender.Name} has successfully defended against {attacker.Name}'s invasion.",
                ImpactStability = 8
            });
        }
        else
        {
            war.Status = WarStatus.Ceasefire;
            war.Result = WarResult.NegotiatedPeace;
            
            if (attacker.MilitaryPower > defender.MilitaryPower)
            {
                war.Reparations = defender.Gdp * 0.03;
                defender.Gdp -= war.Reparations;
            }
            
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = attacker.Id,
                CountryName = attacker.Name,
                Type = "Diplomatic",
                Title = "Peace Treaty Signed",
                Description = $"The war between {attacker.Name} and {defender.Name} has ended with a negotiated peace.",
                ImpactStability = 3
            });
        }
        
        attacker.WarFatigue = Math.Max(0, attacker.WarFatigue - 20);
        defender.WarFatigue = Math.Max(0, defender.WarFatigue - 15);
        
        return true;
    }
}

public class WarTheater
{
    public string Region { get; set; } = string.Empty;
    public string TerrainType { get; set; } = string.Empty;
    public double DefenseBonus { get; set; } = 1.0;
    public double SupplyDifficulty { get; set; } = 1.0;
    public double AirSuperiorityFactor { get; set; } = 1.0;
}
