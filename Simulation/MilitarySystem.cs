using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;

namespace WorldSimApp.Simulation;

public class MilitarySystem
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;

    public MilitarySystem(WorldSimulation simulation)
    {
        _simulation = simulation;
        _random = new Random();
    }

    public void SimulateMilitaryChanges()
    {
        foreach (var country in _simulation.Countries)
        {
            SimulateMilitaryBranches(country);
            SimulateMilitarySpending(country);
            SimulateDefenseIndustry(country);
            SimulateWarFatigue(country);
            SimulateMilitaryReadiness(country);
        }
    }

    private void SimulateMilitaryBranches(Country country)
    {
        double totalPower = country.MilitaryPower;
        double armyRatio = 0.4 + (_random.NextDouble() - 0.5) * 0.1;
        double navyRatio = 0.3 + (_random.NextDouble() - 0.5) * 0.1;
        double airRatio = 1 - armyRatio - navyRatio;

        country.ArmyPower = (int)(totalPower * armyRatio);
        country.NavyPower = (int)(totalPower * navyRatio);
        country.AirPower = (int)(totalPower * airRatio);

        if (country.Region == "Middle East" || country.Region == "Asia")
        {
            country.ArmyPower = (int)(country.ArmyPower * 1.2);
        }
        else if (country.Region == "North America" || country.Region == "Europe")
        {
            country.NavyPower = (int)(country.NavyPower * 1.3);
            country.AirPower = (int)(country.AirPower * 1.2);
        }
    }

    private void SimulateMilitarySpending(Country country)
    {
        country.MilitarySpendingPercent = country.MilitarySpending;
        
        double spending = country.Gdp * (country.MilitarySpendingPercent / 100);
        
        if (country.PoliticalSpectrum == "Right")
            country.MilitarySpendingPercent *= 1.1;

        var activeWars = _simulation.Wars.Count(w => 
            (w.AttackerId == country.Id || w.DefenderId == country.Id) && w.Status == WarStatus.War);
        
        if (activeWars > 0)
        {
            country.MilitarySpendingPercent *= (1 + activeWars * 0.2);
        }

        country.MilitarySpending = country.MilitarySpendingPercent;
    }

    private void SimulateDefenseIndustry(Country country)
    {
        double baseOutput = country.ManufacturingPercent * 0.1;
        double technologyBonus = country.TechnologyPercent * 0.05;
        
        country.DefenseIndustryOutput = baseOutput + technologyBonus + (_random.NextDouble() - 0.5) * 0.5;
        
        if (country.PolicyAgenda.Contains("Military"))
        {
            country.DefenseIndustryOutput *= 1.2;
        }
        
        country.DefenseIndustryOutput *= (1 + country.LaborProductivity - 1);
    }

    private void SimulateWarFatigue(Country country)
    {
        var wars = _simulation.Wars.Where(w => 
            (w.AttackerId == country.Id || w.DefenderId == country.Id) && w.Status == WarStatus.War).ToList();

        foreach (var war in wars)
        {
            int duration = _simulation.CurrentTurn - war.StartTurn;
            if (duration > 5)
            {
                country.WarFatigue = Math.Min(100, country.WarFatigue + (duration - 5) * 2);
                country.Stability = Math.Clamp(country.Stability - (duration - 5) * 0.5, 0, 100);
            }
        }

        if (wars.Count == 0)
        {
            country.WarFatigue = Math.Max(0, country.WarFatigue - 2);
        }
    }

    private void SimulateMilitaryReadiness(Country country)
    {
        double readiness = 70;
        
        readiness += (100 - country.WarFatigue) * 0.2;
        readiness += country.EducationLevel * 0.1;
        readiness += (country.MilitarySpendingPercent - 2) * 2;
        
        readiness += (100 - country.CorruptionIndex) * 0.05;
        
        country.MilitaryReadiness = Math.Clamp(readiness + (_random.NextDouble() - 0.5) * 5, 20, 100);
    }
}
