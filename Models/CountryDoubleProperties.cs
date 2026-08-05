using System;
using System.Collections.Generic;

namespace WorldSimApp.Models;

public static class CountryDoubleProperties
{
    private static readonly List<Func<Country, double>> _getters = new()
    {
        c => c.EconomicGrowth,
        c => c.Happiness,
        c => c.Inflation,
        c => c.Unemployment,
        c => c.InterestRate,
        c => c.BaseInterestRate,
        c => c.TradeBalance,
        c => c.Exports,
        c => c.Imports,
        c => c.ExchangeRate,
        c => c.CurrencyStrength,
        c => c.AgriculturePercent,
        c => c.ManufacturingPercent,
        c => c.ServicesPercent,
        c => c.TechnologyPercent,
        c => c.YouthUnemployment,
        c => c.LaborForce,
        c => c.LaborForceParticipation,
        c => c.GovernmentApproval,
        c => c.MilitarySpending,
        c => c.MilitarySpendingPercent,
        c => c.DefenseIndustryOutput,
        c => c.WarFatigue,
        c => c.MedianAge,
        c => c.AgingIndex,
        c => c.ImmigrationRate,
        c => c.EmigrationRate,
        c => c.NetMigration,
        c => c.EducationLevel,
        c => c.LiteracyRate,
        c => c.CrimeRate,
        c => c.HegemonyDesire,
        c => c.PotentialGdp,
        c => c.RealInterestRate,
        c => c.NAIRU,
        c => c.CurrentAccount,
        c => c.InvestmentRate,
        c => c.SavingsRate,
        c => c.GovernmentSpending,
        c => c.CapacityUtilization,
        c => c.CapitalStock,
        c => c.DepreciationRate,
        c => c.TotalFactorProductivity,
        c => c.GdpPerCapita,
        c => c.LaborForceGrowth,
        c => c.GdpPerCapitaGrowth,
        c => c.PublicOpinion,
        c => c.ScandalLevel,
        c => c.DiplomaticCredibility,
        c => c.FertilityRate,
        c => c.NaturalPopulationGrowth,
        c => c.UrbanizationRate,
        c => c.PensionPressure,
        c => c.LaborProductivity,
        c => c.TertiaryEnrollmentRate,
        c => c.StemGraduatesRate,
        c => c.HealthcareLevel,
        c => c.LifeExpectancy,
        c => c.InfantMortalityRate,
        c => c.HealthcareCost,
        c => c.LawEnforcementSpending,
        c => c.InfrastructureQuality,
        c => c.CorruptionIndex,
        c => c.SocialMobility,
        c => c.IncomeInequality,
        c => c.MiddleClassPercent,
        c => c.MilitaryReadiness,
        c => c.Stability
    };

    private static readonly List<Action<Country, double>> _setters = new()
    {
        (c, v) => c.EconomicGrowth = v,
        (c, v) => c.Happiness = v,
        (c, v) => c.Inflation = v,
        (c, v) => c.Unemployment = v,
        (c, v) => c.InterestRate = v,
        (c, v) => c.BaseInterestRate = v,
        (c, v) => c.TradeBalance = v,
        (c, v) => c.Exports = v,
        (c, v) => c.Imports = v,
        (c, v) => c.ExchangeRate = v,
        (c, v) => c.CurrencyStrength = v,
        (c, v) => c.AgriculturePercent = v,
        (c, v) => c.ManufacturingPercent = v,
        (c, v) => c.ServicesPercent = v,
        (c, v) => c.TechnologyPercent = v,
        (c, v) => c.YouthUnemployment = v,
        (c, v) => c.LaborForce = v,
        (c, v) => c.LaborForceParticipation = v,
        (c, v) => c.GovernmentApproval = v,
        (c, v) => c.MilitarySpending = v,
        (c, v) => c.MilitarySpendingPercent = v,
        (c, v) => c.DefenseIndustryOutput = v,
        (c, v) => c.WarFatigue = v,
        (c, v) => c.MedianAge = v,
        (c, v) => c.AgingIndex = v,
        (c, v) => c.ImmigrationRate = v,
        (c, v) => c.EmigrationRate = v,
        (c, v) => c.NetMigration = v,
        (c, v) => c.EducationLevel = v,
        (c, v) => c.LiteracyRate = v,
        (c, v) => c.CrimeRate = v,
        (c, v) => c.HegemonyDesire = v,
        (c, v) => c.PotentialGdp = v,
        (c, v) => c.RealInterestRate = v,
        (c, v) => c.NAIRU = v,
        (c, v) => c.CurrentAccount = v,
        (c, v) => c.InvestmentRate = v,
        (c, v) => c.SavingsRate = v,
        (c, v) => c.GovernmentSpending = v,
        (c, v) => c.CapacityUtilization = v,
        (c, v) => c.CapitalStock = v,
        (c, v) => c.DepreciationRate = v,
        (c, v) => c.TotalFactorProductivity = v,
        (c, v) => c.GdpPerCapita = v,
        (c, v) => c.LaborForceGrowth = v,
        (c, v) => c.GdpPerCapitaGrowth = v,
        (c, v) => c.PublicOpinion = v,
        (c, v) => c.ScandalLevel = v,
        (c, v) => c.DiplomaticCredibility = v,
        (c, v) => c.FertilityRate = v,
        (c, v) => c.NaturalPopulationGrowth = v,
        (c, v) => c.UrbanizationRate = v,
        (c, v) => c.PensionPressure = v,
        (c, v) => c.LaborProductivity = v,
        (c, v) => c.TertiaryEnrollmentRate = v,
        (c, v) => c.StemGraduatesRate = v,
        (c, v) => c.HealthcareLevel = v,
        (c, v) => c.LifeExpectancy = v,
        (c, v) => c.InfantMortalityRate = v,
        (c, v) => c.HealthcareCost = v,
        (c, v) => c.LawEnforcementSpending = v,
        (c, v) => c.InfrastructureQuality = v,
        (c, v) => c.CorruptionIndex = v,
        (c, v) => c.SocialMobility = v,
        (c, v) => c.IncomeInequality = v,
        (c, v) => c.MiddleClassPercent = v,
        (c, v) => c.MilitaryReadiness = v,
        (c, v) => c.Stability = v
    };

    public static int Count => _getters.Count;

    public static double GetValue(Country country, int index) => _getters[index](country);

    public static void SetValue(Country country, int index, double value) => _setters[index](country, value);

    public static void CleanupInvalidValues(Country country)
    {
        for (int i = 0; i < _getters.Count; i++)
        {
            var value = _getters[i](country);
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                _setters[i](country, 0.0);
            }
        }
    }
}
