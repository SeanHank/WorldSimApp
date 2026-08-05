using System;
using System.Linq;
using WorldSimApp.Models;

namespace WorldSimApp.Simulation;

public class CountryAnalyzer
{
    private readonly WorldSimulation _simulation;
    private readonly GameSettings _settings;
    private readonly Random _random;

    public CountryAnalyzer(WorldSimulation simulation, GameSettings settings)
    {
        _simulation = simulation;
        _settings = settings;
        _random = new Random();
    }

    public void SimulateMilitaryChanges()
    {
        foreach (var country in _simulation.Countries)
        {
            double change = (_random.NextDouble() - 0.5) * 5;
            
            if (country.Stability < 40)
            {
                change -= _random.NextDouble() * 3;
            }
            
            var war = _simulation.Wars.FirstOrDefault(w => w.AttackerId == country.Id || w.DefenderId == country.Id);
            if (war != null)
            {
                change += _random.NextDouble() * 5;
            }
            
            country.MilitaryPower = Math.Max(1, (int)(country.MilitaryPower * (1 + change / 100)));
        }
    }

    public void SimulatePoliticalChanges()
    {
        foreach (var country in _simulation.Countries)
        {
            double stabilityChange = (_random.NextDouble() - 0.5) * 3;
            
            if (country.Happiness < 30)
            {
                stabilityChange -= _random.NextDouble() * 5;
            }
            else if (country.Happiness > 70)
            {
                stabilityChange += _random.NextDouble() * 2;
            }
            
            if (country.EconomicGrowth < -3)
            {
                stabilityChange -= _random.NextDouble() * 3;
            }
            
            var war = _simulation.Wars.FirstOrDefault(w => w.AttackerId == country.Id || w.DefenderId == country.Id);
            if (war != null)
            {
                stabilityChange -= _random.NextDouble() * 5;
            }
            
            country.Stability = Math.Clamp(country.Stability + stabilityChange, 0, 100);
        }
    }

    public void UpdateCountryStats()
    {
        foreach (var country in _simulation.Countries)
        {
            if (country.Stability < 20)
            {
                country.Ideology = "Chaos";
            }
            else if (country.Stability < 40)
            {
                country.Ideology = country.Ideology == "Chaos" ? "Chaos" : "Unstable";
            }
            else if (country.Happiness > 80 && country.Stability > 80)
            {
                country.Ideology = "Prosperous";
            }
            else if (country.MilitaryPower > 1000)
            {
                country.Ideology = "Military State";
            }
        }
    }
}
