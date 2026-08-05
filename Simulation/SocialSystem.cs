using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;

namespace WorldSimApp.Simulation;

public class SocialSystem
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;

    public SocialSystem(WorldSimulation simulation)
    {
        _simulation = simulation;
        _random = new Random();
    }

    public void SimulateSocialChanges()
    {
        foreach (var country in _simulation.Countries)
        {
            SimulateRegionalContagion(country);
            SimulatePopulationPyramid(country);
            SimulateDemographicTransition(country);
            SimulateMigration(country);
            SimulateEducation(country);
            SimulateCrime(country);
            SimulateLaborProductivity(country);
            SimulateHealthcare(country);
            SimulateSocialMobility(country);
        }
    }
    
    private void SimulateRegionalContagion(Country country)
    {
        double contagionFactor = _simulation.Settings.ContagionFactor;
        double randomness = _simulation.Settings.RandomnessMultiplier;
        
        var region = _simulation.GeopoliticalFactors
            .FirstOrDefault(g => g.Region == country.Region);
        
        if (region != null && region.IsContested)
        {
            double contagionRisk = RandomManager.NextGaussian(0.1, 0.05) * contagionFactor;
            if (RandomManager.Chance(contagionRisk * randomness))
            {
                country.Stability -= RandomManager.NextRange(0.5, 2.0) * randomness;
                country.CrimeRate += RandomManager.NextRange(0.2, 1.0) * randomness;
            }
        }
        
        var neighbors = _simulation.Countries
            .Where(c => c.Region == country.Region && c.Id != country.Id)
            .ToList();
        
        foreach (var neighbor in neighbors)
        {
            if (neighbor.EconomicGrowth < -3)
            {
                if (RandomManager.Chance(0.2 * contagionFactor))
                {
                    double spillover = (neighbor.EconomicGrowth - country.EconomicGrowth) * 0.15 * contagionFactor;
                    country.EconomicGrowth += spillover;
                }
            }
            
            if (neighbor.Stability < 40 && RandomManager.Chance(0.1 * contagionFactor))
            {
                country.Stability -= RandomManager.NextRange(0.3, 1.0) * contagionFactor;
            }
            
            if (neighbor.CrimeRate > country.CrimeRate + 5 && RandomManager.Chance(0.15 * contagionFactor))
            {
                country.CrimeRate += (neighbor.CrimeRate - country.CrimeRate) * 0.1 * contagionFactor;
            }
        }
    }

    private void SimulatePopulationPyramid(Country country)
    {
        long total = country.Population;
        
        double age0_14Ratio = CalculateAgeGroupRatio(country.MedianAge, "young");
        double age15_64Ratio = CalculateAgeGroupRatio(country.MedianAge, "working");
        double age65PlusRatio = CalculateAgeGroupRatio(country.MedianAge, "elderly");
        
        country.PopulationUnder18 = (long)(total * age0_14Ratio * 0.85);
        long population18_64 = (long)(total * age15_64Ratio);
        country.PopulationOver60 = (long)(total * age65PlusRatio);
        country.Population18_35 = (long)(population18_64 * 0.35);
        country.Population36_60 = population18_64 - country.Population18_35;
        
        country.AgingIndex = (double)country.PopulationOver60 / Math.Max(1, country.PopulationUnder18);
        country.MedianAge += 0.1;
        
        double childMortality = CalculateChildMortality(country);
        if (childMortality > 0.02)
        {
            country.PopulationUnder18 = (long)(country.PopulationUnder18 * (1 - childMortality));
        }
    }

    private double CalculateAgeGroupRatio(double medianAge, string group)
    {
        return group switch
        {
            "young" => Math.Max(0.1, 0.3 - (medianAge - 30) * 0.008),
            "working" => Math.Min(0.7, 0.6 + (medianAge - 35) * 0.005),
            "elderly" => Math.Min(0.3, 0.1 + (medianAge - 35) * 0.006),
            _ => 0.3
        };
    }

    private double CalculateChildMortality(Country country)
    {
        double baseMortality = 0.015;
        baseMortality -= country.EducationLevel * 0.0002;
        baseMortality -= country.HealthcareLevel * 0.0001;
        
        if (country.Population > 0)
        {
            baseMortality += (country.PopulationUnder18 / (double)country.Population) * 0.01;
        }
        
        return Math.Max(0.002, baseMortality);
    }

    private void SimulateDemographicTransition(Country country)
    {
        double fertilityRate = country.FertilityRate;
        
        if (country.Happiness > 70 && country.EducationLevel > 60)
        {
            fertilityRate *= 1.05;
        }
        
        if (country.Unemployment > 15)
        {
            fertilityRate *= 0.95;
        }
        
        if (country.MedianAge > 40)
        {
            fertilityRate *= 0.97;
        }
        
        if (country.UrbanizationRate > 70)
        {
            fertilityRate *= 0.98;
        }
        
        country.FertilityRate = Math.Clamp(fertilityRate, 1.0, 4.5);
        
        double populationGrowth = (country.FertilityRate - 2.1) * 0.01;
        populationGrowth += _random.NextDouble() * 0.005;
        populationGrowth -= country.AgingIndex * 0.002;
        
        country.NaturalPopulationGrowth = populationGrowth;
        country.Population = (long)(country.Population * (1 + populationGrowth));
        
        if (country.MedianAge > 45 && country.AgingIndex > 1.0)
        {
            country.PensionPressure = Math.Min(100, country.PensionPressure + 2);
            country.LaborForceParticipation = Math.Clamp(country.LaborForceParticipation - 0.5, 40, 80);
        }
    }

    private void SimulateMigration(Country country)
    {
        double pushFactors = 0;
        pushFactors += (100 - country.Happiness) * 0.02;
        pushFactors += country.Unemployment * 0.15;
        pushFactors += country.CrimeRate * 0.1;
        pushFactors += country.PensionPressure * 0.05;
        
        double pullFactors = 0;
        pullFactors += Math.Max(0, country.EconomicGrowth) * 0.3;
        pullFactors += country.EducationLevel * 0.03;
        pullFactors += country.HealthcareLevel * 0.02;
        pullFactors += 1;
        
        double netMigration = pullFactors - pushFactors;
        netMigration += (_random.NextDouble() - 0.5) * 0.8;
        
        double migrationSensitivity = 1.0;
        if (country.Population > 100000000)
            migrationSensitivity = 0.7;
        
        netMigration *= migrationSensitivity;
        
        country.NetMigration = netMigration;
        country.ImmigrationRate = Math.Max(0, 5 + netMigration);
        country.EmigrationRate = Math.Max(0, 5 - netMigration);
        
        country.Population = (long)(country.Population * (1 + country.NetMigration / 1000));
        
        if (country.NetMigration > 2)
        {
            country.UrbanizationRate = Math.Min(95, country.UrbanizationRate + 0.3);
        }
        else if (country.NetMigration < -2)
        {
            country.UrbanizationRate = Math.Max(20, country.UrbanizationRate - 0.2);
        }
    }

    private void SimulateEducation(Country country)
    {
        double baseChange = 0;
        
        double growthFactor = (country.EconomicGrowth - 1) * 0.03;
        baseChange += growthFactor;
        
        double stabilityFactor = (country.Stability - 60) * 0.015;
        baseChange += stabilityFactor;
        
        if (country.EconomicGrowth < -2)
        {
            baseChange -= 0.15;
        }
        
        var activeWar = _simulation.Wars.FirstOrDefault(w => w.AttackerId == country.Id || w.DefenderId == country.Id);
        if (activeWar != null)
        {
            baseChange -= 0.25;
        }
        
        if (country.Stability < 40)
        {
            baseChange -= 0.2;
        }
        
        if (country.CorruptionIndex > 50)
        {
            baseChange -= (country.CorruptionIndex - 50) * 0.005;
        }
        
        double populationGrowthFactor = country.NaturalPopulationGrowth * 2;
        baseChange -= populationGrowthFactor;
        
        if (country.GovernmentSpending < 12)
        {
            baseChange -= (12 - country.GovernmentSpending) * 0.01;
        }
        
        double randomChange = (_random.NextDouble() - 0.5) * 0.12;
        baseChange += randomChange;
        
        country.EducationLevel += baseChange;
        
        double maxEducation = 70 + (country.Gdp / 1e12) * 2;
        maxEducation = Math.Min(maxEducation, 85);
        
        if (country.CorruptionIndex > 60)
            maxEducation -= 10;
        if (country.Stability < 50)
            maxEducation -= 8;
        
        country.EducationLevel = Math.Clamp(country.EducationLevel, 12, maxEducation);

        country.LiteracyRate = 55 + country.EducationLevel * 0.4 + (_random.NextDouble() - 0.5) * 3;
        country.LiteracyRate = Math.Clamp(country.LiteracyRate, 45, 98);
        
        double tertiaryBase = (country.EducationLevel - 25) * 0.5;
        tertiaryBase += (country.Gdp / 1e13) * 2;
        tertiaryBase += (_random.NextDouble() - 0.5) * 4;
        country.TertiaryEnrollmentRate = Math.Clamp(tertiaryBase, 4, 50);
        
        double stemBase = country.TertiaryEnrollmentRate * (country.TechnologyPercent / 55);
        country.StemGraduatesRate = Math.Clamp(stemBase + (_random.NextDouble() - 0.5) * 2, 2, 22);
    }

    private void SimulateLaborProductivity(Country country)
    {
        double productivityBase = 1.0;
        
        productivityBase += country.EducationLevel * 0.01;
        productivityBase += country.TechnologyPercent * 0.008;
        productivityBase += country.LiteracyRate * 0.005;
        productivityBase += country.HealthcareLevel * 0.006;
        
        if (country.InfrastructureQuality > 70)
            productivityBase *= 1.1;
        if (country.CorruptionIndex > 50)
            productivityBase *= 0.9;
        
        productivityBase *= (1 + (_random.NextDouble() - 0.5) * 0.05);
        
        country.LaborProductivity = Math.Clamp(productivityBase, 0.5, 3.0);
        
        double gdpPerCapita = country.Population > 0 ? country.Gdp / (country.Population / 1000000) : 0;
        country.GdpPerCapitaGrowth = (gdpPerCapita * country.LaborProductivity / 1000000) - (country.EconomicGrowth / 10);
    }

    private void SimulateCrime(Country country)
    {
        double baseCrime = 3;
        
        baseCrime += (100 - country.Happiness) * 0.05;
        baseCrime += country.Unemployment * 0.1;
        baseCrime -= country.Stability * 0.03;
        baseCrime -= country.EducationLevel * 0.02;
        
        if (country.PoliticalSpectrum == "Right")
            baseCrime *= 0.9;
        
        country.CrimeRate = Math.Clamp(baseCrime + (_random.NextDouble() - 0.5), 0.5, 20);
        
        country.Stability = Math.Clamp(country.Stability - (country.CrimeRate - 5) * 0.1, 0, 100);
        
        double prisonPopulation = country.CrimeRate * country.Population / 100000;
        country.PrisonPopulation = (long)prisonPopulation;
        
        if (country.CrimeRate > 10)
        {
            country.LawEnforcementSpending = Math.Min(10, country.LawEnforcementSpending + 0.2);
        }
    }

    private void SimulateHealthcare(Country country)
    {
        double baseHealthcare = 50;
        
        baseHealthcare += country.EducationLevel * 0.3;
        baseHealthcare += country.Gdp / 10000000;
        baseHealthcare += (_random.NextDouble() - 0.5) * 5;
        
        country.HealthcareLevel = Math.Clamp(baseHealthcare, 20, 95);
        
        double lifeExpectancy = 50 + country.HealthcareLevel * 0.4 + (_random.NextDouble() - 0.5) * 2;
        country.LifeExpectancy = Math.Clamp(lifeExpectancy, 45, 85);
        
        double infantMortality = 50 - country.HealthcareLevel * 0.4 + (_random.NextDouble() - 0.5) * 2;
        country.InfantMortalityRate = Math.Clamp(infantMortality, 2, 40);
        
        if (country.AgingIndex > 1.0)
        {
            country.HealthcareCost = Math.Min(20, country.HealthcareCost + 0.3);
        }
    }

    private void SimulateSocialMobility(Country country)
    {
        double mobilityBase = 30;
        
        mobilityBase += (100 - country.CorruptionIndex) * 0.3;
        mobilityBase += country.EducationLevel * 0.2;
        mobilityBase += (100 - country.IncomeInequality) * 0.2;
        
        if (country.PolicyAgenda.Contains("Welfare"))
            mobilityBase += 10;
        
        country.SocialMobility = Math.Clamp(mobilityBase + (_random.NextDouble() - 0.5) * 3, 10, 70);
        
        double inequalityChange = 0;
        if (country.EconomicGrowth > 5)
        {
            inequalityChange = _random.NextDouble() * 2;
        }
        else if (country.EconomicGrowth < 0)
        {
            inequalityChange = -_random.NextDouble() * 1.5;
        }
        
        country.IncomeInequality = Math.Clamp(country.IncomeInequality + inequalityChange, 20, 70);
        
        double middleClass = 50 - country.IncomeInequality * 0.3 + (_random.NextDouble() - 0.5) * 2;
        country.MiddleClassPercent = Math.Clamp(middleClass, 20, 70);
    }
}
