using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSimApp.Models;

namespace WorldSimApp.ViewModels;

public partial class MainWindowViewModel
{
    private void UpdateCompareInfo()
    {
        if (CompareCountry1 == null || CompareCountry2 == null)
        {
            CompareInfo = "Select two countries to compare";
            CompareItems.Clear();
            return;
        }
        
        var c1 = CompareCountry1;
        var c2 = CompareCountry2;

        CompareItems.Clear();
        
        CompareItems.Add(new CompareItem { Category = "", Country1Label = c1.Name, Country2Label = c2.Name, Country1Value = "", Country2Value = "" });
        
        CompareItems.Add(new CompareItem { Category = "GOVERNMENT", Country1Label = "Party", Country1Value = c1.RulingParty, Country2Label = "Party", Country2Value = c2.RulingParty });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Spectrum", Country1Value = c1.PoliticalSpectrum, Country2Label = "Spectrum", Country2Value = c2.PoliticalSpectrum });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Approval", Country1Value = $"{c1.GovernmentApproval:F1}%", Country2Label = "Approval", Country2Value = $"{c2.GovernmentApproval:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Next Election", Country1Value = $"Turn {c1.NextElectionTurn}", Country2Label = "Next Election", Country2Value = $"Turn {c2.NextElectionTurn}" });
        
        CompareItems.Add(new CompareItem { Category = "ECONOMY", Country1Label = "GDP", Country1Value = $"${c1.Gdp:N0}", Country2Label = "GDP", Country2Value = $"${c2.Gdp:N0}" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Growth", Country1Value = $"{c1.EconomicGrowth:F1}%", Country2Label = "Growth", Country2Value = $"{c2.EconomicGrowth:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Interest Rate", Country1Value = $"{c1.InterestRate:F2}%", Country2Label = "Interest Rate", Country2Value = $"{c2.InterestRate:F2}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Inflation", Country1Value = $"{c1.Inflation:F1}%", Country2Label = "Inflation", Country2Value = $"{c2.Inflation:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Trade Balance", Country1Value = $"${c1.TradeBalance:N0}", Country2Label = "Trade Balance", Country2Value = $"${c2.TradeBalance:N0}" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Currency", Country1Value = $"{c1.CurrencyStrength:F1}", Country2Label = "Currency", Country2Value = $"{c2.CurrencyStrength:F1}" });
        
        CompareItems.Add(new CompareItem { Category = "POPULATION", Country1Label = "Total", Country1Value = c1.Population.ToString("N0"), Country2Label = "Total", Country2Value = c2.Population.ToString("N0") });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Under 18", Country1Value = c1.PopulationUnder18.ToString("N0"), Country2Label = "Under 18", Country2Value = c2.PopulationUnder18.ToString("N0") });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Over 60", Country1Value = c1.PopulationOver60.ToString("N0"), Country2Label = "Over 60", Country2Value = c2.PopulationOver60.ToString("N0") });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Median Age", Country1Value = $"{c1.MedianAge:F1}", Country2Label = "Median Age", Country2Value = $"{c2.MedianAge:F1}" });
        
        CompareItems.Add(new CompareItem { Category = "LABOR & EDUCATION", Country1Label = "Unemployment", Country1Value = $"{c1.Unemployment:F1}%", Country2Label = "Unemployment", Country2Value = $"{c2.Unemployment:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Youth Unemp", Country1Value = $"{c1.YouthUnemployment:F1}%", Country2Label = "Youth Unemp", Country2Value = $"{c2.YouthUnemployment:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Education", Country1Value = $"{c1.EducationLevel:F1}", Country2Label = "Education", Country2Value = $"{c2.EducationLevel:F1}" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Literacy", Country1Value = $"{c1.LiteracyRate:F1}%", Country2Label = "Literacy", Country2Value = $"{c2.LiteracyRate:F1}%" });
        
        CompareItems.Add(new CompareItem { Category = "MILITARY", Country1Label = "Total Power", Country1Value = c1.MilitaryPower.ToString("N0"), Country2Label = "Total Power", Country2Value = c2.MilitaryPower.ToString("N0") });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Army", Country1Value = c1.ArmyPower.ToString("N0"), Country2Label = "Army", Country2Value = c2.ArmyPower.ToString("N0") });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Navy", Country1Value = c1.NavyPower.ToString("N0"), Country2Label = "Navy", Country2Value = c2.NavyPower.ToString("N0") });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Air", Country1Value = c1.AirPower.ToString("N0"), Country2Label = "Air", Country2Value = c2.AirPower.ToString("N0") });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Spending", Country1Value = $"{c1.MilitarySpendingPercent:F1}%", Country2Label = "Spending", Country2Value = $"{c2.MilitarySpendingPercent:F1}%" });
        
        CompareItems.Add(new CompareItem { Category = "INDUSTRY SECTORS", Country1Label = "Agriculture", Country1Value = $"{c1.AgriculturePercent:F1}%", Country2Label = "Agriculture", Country2Value = $"{c2.AgriculturePercent:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Manufacturing", Country1Value = $"{c1.ManufacturingPercent:F1}%", Country2Label = "Manufacturing", Country2Value = $"{c2.ManufacturingPercent:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Services", Country1Value = $"{c1.ServicesPercent:F1}%", Country2Label = "Services", Country2Value = $"{c2.ServicesPercent:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Technology", Country1Value = $"{c1.TechnologyPercent:F1}%", Country2Label = "Technology", Country2Value = $"{c2.TechnologyPercent:F1}%" });
        
        CompareItems.Add(new CompareItem { Category = "DIPLOMACY", Country1Label = "Allies", Country1Value = c1.Allies.Count.ToString(), Country2Label = "Allies", Country2Value = c2.Allies.Count.ToString() });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Enemies", Country1Value = c1.Enemies.Count.ToString(), Country2Label = "Enemies", Country2Value = c2.Enemies.Count.ToString() });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Sanctions", Country1Value = c1.ActiveSanctions.Count.ToString(), Country2Label = "Sanctions", Country2Value = c2.ActiveSanctions.Count.ToString() });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Regional Power", Country1Value = c1.RegionalPower, Country2Label = "Regional Power", Country2Value = c2.RegionalPower });
        
        CompareItems.Add(new CompareItem { Category = "SOCIAL", Country1Label = "Happiness", Country1Value = $"{c1.Happiness:F1}%", Country2Label = "Happiness", Country2Value = $"{c2.Happiness:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Stability", Country1Value = $"{c1.Stability:F1}%", Country2Label = "Stability", Country2Value = $"{c2.Stability:F1}%" });
        CompareItems.Add(new CompareItem { Category = "", Country1Label = "Culture", Country1Value = c1.CultureGroup, Country2Label = "Culture", Country2Value = c2.CultureGroup });
        
        CompareInfo = $"""
╔══════════════════════════════════════════════════════════════════════════════════════════════════╗
║                              COUNTRY COMPARISON                                                ║
╠══════════════════════════════════════════════════════════════════════════════════════════════════╣
║ {c1.Name,-22} │ {c2.Name,-22} ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ 🏛️ GOVERNMENT                           │ 🏛️ GOVERNMENT                                 ║
║   Party: {c1.RulingParty,-18} │   Party: {c2.RulingParty,-18} ║
║   Spectrum: {c1.PoliticalSpectrum,-14} │   Spectrum: {c2.PoliticalSpectrum,-14} ║
║   Approval: {c1.GovernmentApproval,-14:F1}% │   Approval: {c2.GovernmentApproval,-14:F1}% ║
║   Election: Turn {c1.NextElectionTurn,-12} │   Election: Turn {c2.NextElectionTurn,-12} ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ 💰 ECONOMY                              │ 💰 ECONOMY                                    ║
║   GDP: ${c1.Gdp,-18:N0} │   GDP: ${c2.Gdp,-18:N0} ║
║   Growth: {c1.EconomicGrowth,-17:F1}% │   Growth: {c2.EconomicGrowth,-17:F1}% ║
║   Interest Rate: {c1.InterestRate,-11:F2}% │   Interest Rate: {c2.InterestRate,-11:F2}% ║
║   Inflation: {c1.Inflation,-15:F1}% │   Inflation: {c2.Inflation,-15:F1}% ║
║   Trade Balance: ${c1.TradeBalance,-11:N0} │   Trade Balance: ${c2.TradeBalance,-11:N0} ║
║   Currency: {c1.CurrencyStrength,-16:F1} │   Currency: {c2.CurrencyStrength,-16:F1} ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ 👥 POPULATION                           │ 👥 POPULATION                                 ║
║   Total: {c1.Population,-19:N0} │   Total: {c2.Population,-19:N0} ║
║   Under 18: {c1.PopulationUnder18,-15:N0} │   Under 18: {c2.PopulationUnder18,-15:N0} ║
║   Over 60: {c1.PopulationOver60,-16:N0} │   Over 60: {c2.PopulationOver60,-16:N0} ║
║   Median Age: {c1.MedianAge,-14:F1} │   Median Age: {c2.MedianAge,-14:F1} ║
║   Immigration: {c1.ImmigrationRate,-12:F1}% │   Immigration: {c2.ImmigrationRate,-12:F1}% ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ 💼 LABOR & EDUCATION                    │ 💼 LABOR & EDUCATION                          ║
║   Unemployment: {c1.Unemployment,-12:F1}% │   Unemployment: {c2.Unemployment,-12:F1}% ║
║   Youth Unemp: {c1.YouthUnemployment,-11:F1}% │   Youth Unemp: {c2.YouthUnemployment,-11:F1}% ║
║   Education: {c1.EducationLevel,-15:F1} │   Education: {c2.EducationLevel,-15:F1} ║
║   Literacy: {c1.LiteracyRate,-15:F1}% │   Literacy: {c2.LiteracyRate,-15:F1}% ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ ⚔️ MILITARY                             │ ⚔️ MILITARY                                   ║
║   Total Power: {c1.MilitaryPower,-13} │   Total Power: {c2.MilitaryPower,-13} ║
║   Army: {c1.ArmyPower,-18} │   Army: {c2.ArmyPower,-18} ║
║   Navy: {c1.NavyPower,-18} │   Navy: {c2.NavyPower,-18} ║
║   Air: {c1.AirPower,-19} │   Air: {c2.AirPower,-19} ║
║   Spending: {c1.MilitarySpendingPercent,-13:F1}% │   Spending: {c2.MilitarySpendingPercent,-13:F1}% ║
║   War Fatigue: {c1.WarFatigue,-13:F1}% │   War Fatigue: {c2.WarFatigue,-13:F1}% ║
║   Defense Ind: {c1.DefenseIndustryOutput,-11:F1}% │   Defense Ind: {c2.DefenseIndustryOutput,-11:F1}% ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ 🏭 INDUSTRY SECTORS                     │ 🏭 INDUSTRY SECTORS                           ║
║   Agriculture: {c1.AgriculturePercent,-11:F1}% │   Agriculture: {c2.AgriculturePercent,-11:F1}% ║
║   Manufacturing: {c1.ManufacturingPercent,-8:F1}% │   Manufacturing: {c2.ManufacturingPercent,-8:F1}% ║
║   Services: {c1.ServicesPercent,-13:F1}% │   Services: {c2.ServicesPercent,-13:F1}% ║
║   Technology: {c1.TechnologyPercent,-12:F1}% │   Technology: {c2.TechnologyPercent,-12:F1}% ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ 🌍 DIPLOMACY                            │ 🌍 DIPLOMACY                                  ║
║   Allies: {c1.Allies.Count,-18} │   Allies: {c2.Allies.Count,-18} ║
║   Enemies: {c1.Enemies.Count,-17} │   Enemies: {c2.Enemies.Count,-17} ║
║   Sanctions: {c1.ActiveSanctions.Count,-14} │   Sanctions: {c2.ActiveSanctions.Count,-14} ║
║   Treaties: {c1.Treaties.Count,-16} │   Treaties: {c2.Treaties.Count,-16} ║
║   Regional Power: {c1.RegionalPower,-10} │   Regional Power: {c2.RegionalPower,-10} ║
║   Hegemony Desire: {c1.HegemonyDesire,-9:F2} │   Hegemony Desire: {c2.HegemonyDesire,-9:F2} ║
╠════════════════════════════════════════════╦══════════════════════════════════════════════════╣
║ 😊 SOCIAL                               │ 😊 SOCIAL                                     ║
║   Happiness: {c1.Happiness,-15:F1}% │   Happiness: {c2.Happiness,-15:F1}% ║
║   Stability: {c1.Stability,-16:F1}% │   Stability: {c2.Stability,-16:F1}% ║
║   Crime Rate: {c1.CrimeRate,-14:F1} │   Crime Rate: {c2.CrimeRate,-14:F1} ║
║   Culture: {c1.CultureGroup,-17} │   Culture: {c2.CultureGroup,-17} ║
║   Religion: {c1.DominantReligion,-15} │   Religion: {c2.DominantReligion,-15} ║
╚════════════════════════════════════════════╩══════════════════════════════════════════════════╝
""";
    }

    private void UpdateSelectedCountryInfo()
    {
        if (SelectedCountry == null) return;
        
        var c = _simulation.GetCountry(SelectedCountry.Id);
        if (c == null) return;
        
        SelectedCountry = c;
        
        var resources = _simulation.GetCountryResources(c.Id);
        var orgs = _simulation.GetCountryOrganizations(c.Id);
        var routes = _simulation.GetCountryTradeRoutes(c.Id);
        
        var allies = c.Allies.Count > 0 ? string.Join(", ", GetCountryNames(c.Allies)) : "None";
        var enemies = c.Enemies.Count > 0 ? string.Join(", ", GetCountryNames(c.Enemies)) : "None";

        CountryDetails.Clear();

        CountryDetails.Add(new CountryDetailItem { Category = "BASIC", Property = "Country", Value = $"{c.Name} ({c.Code})" });
        CountryDetails.Add(new CountryDetailItem { Category = "BASIC", Property = "Region", Value = c.Region });
        CountryDetails.Add(new CountryDetailItem { Category = "BASIC", Property = "Ideology", Value = c.Ideology });
        CountryDetails.Add(new CountryDetailItem { Category = "BASIC", Property = "Culture", Value = c.CultureGroup });
        CountryDetails.Add(new CountryDetailItem { Category = "BASIC", Property = "Religion", Value = c.DominantReligion });

        CountryDetails.Add(new CountryDetailItem { Category = "GOVERNMENT", Property = "Ruling Party", Value = c.RulingParty });
        CountryDetails.Add(new CountryDetailItem { Category = "GOVERNMENT", Property = "Political Spectrum", Value = c.PoliticalSpectrum });
        CountryDetails.Add(new CountryDetailItem { Category = "GOVERNMENT", Property = "Government Approval", Value = $"{c.GovernmentApproval:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "GOVERNMENT", Property = "Next Election", Value = $"Turn {c.NextElectionTurn}" });
        CountryDetails.Add(new CountryDetailItem { Category = "GOVERNMENT", Property = "Policy Agenda", Value = string.Join(", ", c.PolicyAgenda) });
        CountryDetails.Add(new CountryDetailItem { Category = "GOVERNMENT", Property = "Regional Status", Value = c.RegionalPower });

        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "GDP", Value = $"${c.Gdp:N0}" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Economic Growth", Value = $"{c.EconomicGrowth:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Interest Rate", Value = $"{c.InterestRate:F2}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Inflation", Value = $"{c.Inflation:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Exports", Value = $"${c.Exports:N0}" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Imports", Value = $"${c.Imports:N0}" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Trade Balance", Value = $"${c.TradeBalance:N0}" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Currency Strength", Value = $"{c.CurrencyStrength:F1}" });
        CountryDetails.Add(new CountryDetailItem { Category = "ECONOMY", Property = "Exchange Rate", Value = $"{c.ExchangeRate:F3}" });

        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Total Population", Value = c.Population.ToString("N0") });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Under 18", Value = $"{c.PopulationUnder18:N0} ({(double)c.PopulationUnder18/c.Population*100:F1}%)" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "18-35", Value = $"{c.Population18_35:N0} ({(double)c.Population18_35/c.Population*100:F1}%)" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "36-60", Value = $"{c.Population36_60:N0} ({(double)c.Population36_60/c.Population*100:F1}%)" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Over 60", Value = $"{c.PopulationOver60:N0} ({(double)c.PopulationOver60/c.Population*100:F1}%)" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Median Age", Value = $"{c.MedianAge:F1}" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Aging Index", Value = $"{c.AgingIndex:F2}" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Net Migration", Value = $"{c.NetMigration:F2}%/year" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Immigration Rate", Value = $"{c.ImmigrationRate:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "POPULATION", Property = "Emigration Rate", Value = $"{c.EmigrationRate:F1}%" });

        CountryDetails.Add(new CountryDetailItem { Category = "LABOR & EDUCATION", Property = "Labor Force", Value = c.LaborForce.ToString("N0") });
        CountryDetails.Add(new CountryDetailItem { Category = "LABOR & EDUCATION", Property = "Participation Rate", Value = $"{c.LaborForceParticipation:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "LABOR & EDUCATION", Property = "Unemployment", Value = $"{c.Unemployment:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "LABOR & EDUCATION", Property = "Youth Unemployment", Value = $"{c.YouthUnemployment:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "LABOR & EDUCATION", Property = "Education Level", Value = $"{c.EducationLevel:F1}" });
        CountryDetails.Add(new CountryDetailItem { Category = "LABOR & EDUCATION", Property = "Literacy Rate", Value = $"{c.LiteracyRate:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "LABOR & EDUCATION", Property = "Crime Rate", Value = $"{c.CrimeRate:F1}/1000" });

        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "Total Power", Value = c.MilitaryPower.ToString("N0") });
        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "Army", Value = $"{c.ArmyPower:N0} ({(double)c.ArmyPower/c.MilitaryPower*100:F0}%)" });
        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "Navy", Value = $"{c.NavyPower:N0} ({(double)c.NavyPower/c.MilitaryPower*100:F0}%)" });
        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "Air", Value = $"{c.AirPower:N0} ({(double)c.AirPower/c.MilitaryPower*100:F0}%)" });
        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "Military Spending", Value = $"{c.MilitarySpendingPercent:F1}% of GDP" });
        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "Defense Industry", Value = $"{c.DefenseIndustryOutput:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "War Fatigue", Value = $"{c.WarFatigue:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "MILITARY", Property = "Wars Fought", Value = $"{c.WarsFought} (Won: {c.WarsWon})" });

        CountryDetails.Add(new CountryDetailItem { Category = "INDUSTRY", Property = "Agriculture", Value = $"{c.AgriculturePercent:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "INDUSTRY", Property = "Manufacturing", Value = $"{c.ManufacturingPercent:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "INDUSTRY", Property = "Services", Value = $"{c.ServicesPercent:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "INDUSTRY", Property = "Technology", Value = $"{c.TechnologyPercent:F1}%" });

        CountryDetails.Add(new CountryDetailItem { Category = "DIPLOMACY", Property = "Allies", Value = allies });
        CountryDetails.Add(new CountryDetailItem { Category = "DIPLOMACY", Property = "Enemies", Value = enemies });
        CountryDetails.Add(new CountryDetailItem { Category = "DIPLOMACY", Property = "Sanctions Applied", Value = c.ActiveSanctions.Count.ToString() });
        CountryDetails.Add(new CountryDetailItem { Category = "DIPLOMACY", Property = "Sanctioned By", Value = c.SanctionedBy.Count.ToString() });
        CountryDetails.Add(new CountryDetailItem { Category = "DIPLOMACY", Property = "Trade Agreements", Value = c.TradeAgreements.Count.ToString() });
        CountryDetails.Add(new CountryDetailItem { Category = "DIPLOMACY", Property = "Treaties", Value = c.Treaties.Count.ToString() });
        CountryDetails.Add(new CountryDetailItem { Category = "DIPLOMACY", Property = "Hegemony Desire", Value = $"{c.HegemonyDesire:F2}" });

        CountryDetails.Add(new CountryDetailItem { Category = "SOCIAL", Property = "Happiness", Value = $"{c.Happiness:F1}%" });
        CountryDetails.Add(new CountryDetailItem { Category = "SOCIAL", Property = "Stability", Value = $"{c.Stability:F1}%" });

        CountryDetails.Add(new CountryDetailItem { Category = "TRADE", Property = "Active Routes", Value = routes.Count.ToString() });

        SelectedCountryInfo = "";
    }

    private void UpdateChartData()
    {
        var worldStability = _simulation.WorldStabilityHistory;
        
        if (worldStability.Count == 0)
        {
            ChartData = "No data available yet";
            return;
        }
        
        CurrentTurn = _simulation.CurrentTurn;
        
        var latest = worldStability.Last();
        var avg = worldStability.Average();
        
        var topGdp = _simulation.Countries.OrderByDescending(c => c.Gdp).Take(5).ToList();
        var topMilitary = _simulation.Countries.OrderByDescending(c => c.MilitaryPower).Take(5).ToList();
        var topHappiness = _simulation.Countries.OrderByDescending(c => c.Happiness).Take(5).ToList();
        var topTech = _simulation.Countries.OrderByDescending(c => c.TechnologyPercent).Take(5).ToList();
        
        var avgInflation = _simulation.Countries.Average(c => c.Inflation);
        var avgUnemployment = _simulation.Countries.Average(c => c.Unemployment);
        var worldGrowth = _simulation.CalculateWorldGdpGrowth();
        var totalPop = _simulation.Countries.Sum(c => c.Population);
        
        var wars = _simulation.Wars.Where(w => w.Status == WarStatus.War).ToList();
        
        var factions = _simulation.Countries
            .GroupBy(c => c.PoliticalSpectrum)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => $"{g.Key}: {g.Count()}")
            .ToList();

        TopGdpCountries.Clear();
        for (int i = 0; i < topGdp.Count; i++)
        {
            TopGdpCountries.Add(new CountryRankItem
            {
                Rank = i + 1,
                Name = topGdp[i].Name,
                Code = topGdp[i].Code,
                Value = $"${topGdp[i].Gdp:N0}",
                SubValue = $"{topGdp[i].EconomicGrowth:+0.0;-0.0}%"
            });
        }

        TopMilitaryCountries.Clear();
        for (int i = 0; i < topMilitary.Count; i++)
        {
            TopMilitaryCountries.Add(new CountryRankItem
            {
                Rank = i + 1,
                Name = topMilitary[i].Name,
                Code = topMilitary[i].Code,
                Value = topMilitary[i].MilitaryPower.ToString("N0")
            });
        }

        TopHappinessCountries.Clear();
        for (int i = 0; i < topHappiness.Count; i++)
        {
            TopHappinessCountries.Add(new CountryRankItem
            {
                Rank = i + 1,
                Name = topHappiness[i].Name,
                Code = topHappiness[i].Code,
                Value = $"{topHappiness[i].Happiness:F1}%"
            });
        }

        TopTechCountries.Clear();
        for (int i = 0; i < topTech.Count; i++)
        {
            TopTechCountries.Add(new CountryRankItem
            {
                Rank = i + 1,
                Name = topTech[i].Name,
                Code = topTech[i].Code,
                Value = $"{topTech[i].TechnologyPercent:F1}%"
            });
        }

        WorldOverview.Clear();
        WorldOverview.Add(new WorldOverviewItem { Metric = "Total Countries", Value = _simulation.Countries.Count.ToString() });
        WorldOverview.Add(new WorldOverviewItem { Metric = "Total Population", Value = totalPop.ToString("N0") });
        WorldOverview.Add(new WorldOverviewItem { Metric = "World Stability", Value = $"{latest:F1}% (Avg: {avg:F1}%)" });
        WorldOverview.Add(new WorldOverviewItem { Metric = "Active Wars", Value = wars.Count.ToString() });
        WorldOverview.Add(new WorldOverviewItem { Metric = "World GDP Growth", Value = $"{worldGrowth:F1}%" });
        WorldOverview.Add(new WorldOverviewItem { Metric = "Avg Inflation", Value = $"{avgInflation:F1}%" });
        WorldOverview.Add(new WorldOverviewItem { Metric = "Avg Unemployment", Value = $"{avgUnemployment:F1}%" });

        IndustryBreakdown.Clear();
        IndustryBreakdown.Add(new GlobalStatisticItem { Category = "Agriculture", Value = $"{_simulation.Countries.Average(c => c.AgriculturePercent):F1}%" });
        IndustryBreakdown.Add(new GlobalStatisticItem { Category = "Manufacturing", Value = $"{_simulation.Countries.Average(c => c.ManufacturingPercent):F1}%" });
        IndustryBreakdown.Add(new GlobalStatisticItem { Category = "Services", Value = $"{_simulation.Countries.Average(c => c.ServicesPercent):F1}%" });
        IndustryBreakdown.Add(new GlobalStatisticItem { Category = "Technology", Value = $"{_simulation.Countries.Average(c => c.TechnologyPercent):F1}%" });

        PopulationTrends.Clear();
        PopulationTrends.Add(new GlobalStatisticItem { Category = "Total Under 18", Value = _simulation.Countries.Sum(c => c.PopulationUnder18).ToString("N0") });
        PopulationTrends.Add(new GlobalStatisticItem { Category = "Total Over 60", Value = _simulation.Countries.Sum(c => c.PopulationOver60).ToString("N0") });
        PopulationTrends.Add(new GlobalStatisticItem { Category = "Avg Median Age", Value = $"{_simulation.Countries.Average(c => c.MedianAge):F1}" });
        PopulationTrends.Add(new GlobalStatisticItem { Category = "Avg Education", Value = $"{_simulation.Countries.Average(c => c.EducationLevel):F1}" });

        Conflicts.Clear();
        foreach (var w in wars)
        {
            var attacker = _simulation.GetCountry(w.AttackerId);
            var defender = _simulation.GetCountry(w.DefenderId);
            Conflicts.Add(new ConflictItem
            {
                Name = w.Name,
                Attacker = attacker?.Name ?? w.AttackerId,
                Defender = defender?.Name ?? w.DefenderId,
                StartTurn = w.StartTurn.ToString(),
                Duration = $"{w.CurrentTurn - w.StartTurn} turns",
                Casualties = (w.AttackerDeaths + w.DefenderDeaths).ToString("N0")
            });
        }

        PoliticalSpectrum.Clear();
        foreach (var f in factions) PoliticalSpectrum.Add(f);

        DiplomaticStatus.Clear();
        DiplomaticStatus.Add($"Total Alliances: {_simulation.Countries.Sum(c => c.Allies.Count) / 2}");
        DiplomaticStatus.Add($"Total Trade Agreements: {_simulation.Countries.Sum(c => c.TradeAgreements.Count) / 2}");
        DiplomaticStatus.Add($"Active Sanctions: {_simulation.Countries.Sum(c => c.ActiveSanctions.Count)}");
        
        ChartData = $"""
╔══════════════════════════════════════════════════════════════════════════════════════════╗
║                           WORLD STATISTICS - Turn {_simulation.CurrentTurn,-4}                                  ║
╠══════════════════════════════════════════════════════════════════════════════════════════╣

🌍 GLOBAL OVERVIEW
  Total Countries: {_simulation.Countries.Count}
  Total Population: {totalPop:N0}
  World Stability: {latest:F1}% (Avg: {avg:F1}%)
  Active Wars: {wars.Count}
  World GDP Growth: {worldGrowth:F1}% | Avg Inflation: {avgInflation:F1}%
  Avg Unemployment: {avgUnemployment:F1}%

⚔️ MILITARY
  Top 5 Powers:
{string.Join("\n", topMilitary.Select((c, i) => $"    {i+1}. {c.Name}: {c.MilitaryPower:N0}"))}

💰 ECONOMY
  Top 5 GDP:
{string.Join("\n", topGdp.Select((c, i) => $"    {i+1}. {c.Name}: ${c.Gdp:N0}"))}

😊 HAPPINESS
  Top 5 Happiest:
{string.Join("\n", topHappiness.Select((c, i) => $"    {i+1}. {c.Name}: {c.Happiness:F1}%"))}

🔬 TECHNOLOGY
  Top 5 Tech Leaders:
{string.Join("\n", topTech.Select((c, i) => $"    {i+1}. {c.Name}: {c.TechnologyPercent:F1}%"))}

🏭 INDUSTRY BREAKDOWN
  Avg Agriculture: {_simulation.Countries.Average(c => c.AgriculturePercent):F1}%
  Avg Manufacturing: {_simulation.Countries.Average(c => c.ManufacturingPercent):F1}%
  Avg Services: {_simulation.Countries.Average(c => c.ServicesPercent):F1}%
  Avg Technology: {_simulation.Countries.Average(c => c.TechnologyPercent):F1}%

👥 POPULATION TRENDS
  Total Under 18: {_simulation.Countries.Sum(c => c.PopulationUnder18):N0}
  Total Over 60: {_simulation.Countries.Sum(c => c.PopulationOver60):N0}
  Avg Median Age: {_simulation.Countries.Average(c => c.MedianAge):F1}
  Avg Education: {_simulation.Countries.Average(c => c.EducationLevel):F1}

⚔️ CONFLICTS
{(wars.Count == 0 ? "  No active wars" : string.Join("\n", wars.Select(w => $"  ⚔️ {w.Name} (Started: Turn {w.StartTurn})")))}

🏛️ POLITICAL SPECTRUM
{string.Join("\n", factions.Select(f => $"  {f}"))}

🌍 DIPLOMATIC STATUS
  Total Alliances: {_simulation.Countries.Sum(c => c.Allies.Count) / 2}
  Total Trade Agreements: {_simulation.Countries.Sum(c => c.TradeAgreements.Count) / 2}
  Active Sanctions: {_simulation.Countries.Sum(c => c.ActiveSanctions.Count)}
╚══════════════════════════════════════════════════════════════════════════════════════════╝
""";
    }

    private void UpdateTradeInfo()
    {
        var routes = _simulation.TradeRoutes;
        
        bool hasSimulationStarted = _simulation.CurrentTurn >= 1;
        
        double totalValue = hasSimulationStarted ? routes.Sum(r => r.Value) : 0;
        
        var topTradersData = _simulation.Countries
            .Select(c => new {
                Country = c,
                ExportValue = hasSimulationStarted ? routes.Where(r => r.ExporterId == c.Id).Sum(r => r.Value) : 0,
                ImportValue = hasSimulationStarted ? routes.Where(r => r.ImporterId == c.Id).Sum(r => r.Value) : 0
            })
            .OrderByDescending(x => x.ExportValue + x.ImportValue)
            .Take(5)
            .ToList();
        
        TopTraders.Clear();
        for (int i = 0; i < topTradersData.Count; i++)
        {
            TopTraders.Add(new CountryRankItem
            {
                Rank = i + 1,
                Name = topTradersData[i].Country.Name,
                Value = $"${topTradersData[i].ExportValue + topTradersData[i].ImportValue:N0}"
            });
        }

        TradeRoutes.Clear();
        foreach (var r in routes)
        {
            var exporter = _simulation.GetCountry(r.ExporterId);
            var importer = _simulation.GetCountry(r.ImporterId);
            double displayValue = hasSimulationStarted ? r.Value : 0;
            TradeRoutes.Add(new TradeRouteItem
            {
                Exporter = exporter?.Name ?? r.ExporterId,
                Importer = importer?.Name ?? r.ImporterId,
                Value = $"${displayValue:N0}",
                Resource = r.ResourceId
            });
        }
        
        var routeDetails = routes.Count > 0
            ? string.Join("\n", routes.Take(10).Select(r => {
                var exporter = _simulation.GetCountry(r.ExporterId);
                var importer = _simulation.GetCountry(r.ImporterId);
                double displayValue = hasSimulationStarted ? r.Value : 0;
                return $"  {exporter?.Name ?? r.ExporterId} -> {importer?.Name ?? r.ImporterId}: ${displayValue:N0}";
            }))
            : "  No trade routes";
        
        TradeInfo = $"""
            Trade Network
            
            Total Trade Volume: ${totalValue:N0}
            Active Trade Routes: {routes.Count}
            
            Top Trading Nations:
            {string.Join("\n", topTradersData.Select(t => $"  {t.Country.Name}: Exp ${t.ExportValue:N0} / Imp ${t.ImportValue:N0}"))}
            
            Recent Trade Routes:
            {routeDetails}
            """;
    }

    private void UpdateEconomyInfo()
    {
        var countries = _simulation.Countries;
        
        var totalGdp = countries.Sum(c => c.Gdp);
        var totalExports = countries.Sum(c => c.Exports);
        var totalImports = countries.Sum(c => c.Imports);
        var avgInflation = countries.Average(c => c.Inflation);
        var worldGrowth = _simulation.CalculateWorldGdpGrowth();
        var avgInterest = countries.Average(c => c.InterestRate);
        
        var topGdp = countries.OrderByDescending(c => c.Gdp).Take(5).ToList();
        var topExports = countries.OrderByDescending(c => c.Exports).Take(5).ToList();
        
        var topTech = countries
            .Select(c => new {
                Country = c,
                TechOutput = c.Gdp * c.TechnologyPercent / 100
            })
            .OrderByDescending(x => x.TechOutput)
            .Take(5)
            .ToList();
        
        double worldTech = countries.Sum(c => c.Gdp * c.TechnologyPercent / 100);
        
        var topManufacturers = countries
            .Select(c => new {
                Country = c,
                ManufacturingOutput = c.Gdp * c.ManufacturingPercent / 100
            })
            .OrderByDescending(x => x.ManufacturingOutput)
            .Take(5)
            .ToList();
        
        double worldManufacturing = countries.Sum(c => c.Gdp * c.ManufacturingPercent / 100);

        GlobalIndicators.Clear();
        GlobalIndicators.Add(new GlobalStatisticItem { Category = "World GDP", Value = $"${totalGdp:N0}" });
        GlobalIndicators.Add(new GlobalStatisticItem { Category = "Total Exports", Value = $"${totalExports:N0}" });
        GlobalIndicators.Add(new GlobalStatisticItem { Category = "Total Imports", Value = $"${totalImports:N0}" });
        GlobalIndicators.Add(new GlobalStatisticItem { Category = "Net Trade Balance", Value = $"${totalExports - totalImports:N0}" });
        GlobalIndicators.Add(new GlobalStatisticItem { Category = "World GDP Growth", Value = $"{worldGrowth:F1}%" });
        GlobalIndicators.Add(new GlobalStatisticItem { Category = "Avg Inflation", Value = $"{avgInflation:F1}%" });
        GlobalIndicators.Add(new GlobalStatisticItem { Category = "Avg Interest Rate", Value = $"{avgInterest:F2}%" });

        IndustrySectors.Clear();
        IndustrySectors.Add(new GlobalStatisticItem { Category = "Agriculture", Value = $"{countries.Average(c => c.AgriculturePercent):F1}%" });
        IndustrySectors.Add(new GlobalStatisticItem { Category = "Manufacturing", Value = $"{countries.Average(c => c.ManufacturingPercent):F1}%" });
        IndustrySectors.Add(new GlobalStatisticItem { Category = "Services", Value = $"{countries.Average(c => c.ServicesPercent):F1}%" });
        IndustrySectors.Add(new GlobalStatisticItem { Category = "Technology", Value = $"{countries.Average(c => c.TechnologyPercent):F1}%" });

        TopEconomies.Clear();
        for (int i = 0; i < topGdp.Count; i++)
        {
            TopEconomies.Add(new EconomyItem
            {
                Rank = i + 1,
                Country = topGdp[i].Name,
                Gdp = $"${topGdp[i].Gdp:N0}",
                Growth = $"{topGdp[i].EconomicGrowth:+0.0;-0.0}%",
                Exports = $"${topGdp[i].Exports:N0}",
                TechPercent = $"{topGdp[i].TechnologyPercent:F1}%",
                CurrencyStrength = $"{topGdp[i].CurrencyStrength:F1}",
                ManufacturingPercent = $"{topGdp[i].ManufacturingPercent:F1}%"
            });
        }

        TopExporters.Clear();
        for (int i = 0; i < topExports.Count; i++)
        {
            TopExporters.Add(new EconomyItem
            {
                Rank = i + 1,
                Country = topExports[i].Name,
                Exports = $"${topExports[i].Exports:N0}"
            });
        }

        TopTechSectors.Clear();
        for (int i = 0; i < topTech.Count; i++)
        {
            double worldShare = worldTech > 0 ? topTech[i].TechOutput / worldTech * 100 : 0;
            TopTechSectors.Add(new EconomyItem
            {
                Rank = i + 1,
                Country = topTech[i].Country.Name,
                TechPercent = $"{worldShare:F1}%"
            });
        }

        TopManufacturers.Clear();
        for (int i = 0; i < topManufacturers.Count; i++)
        {
            double worldShare = worldManufacturing > 0 
                ? topManufacturers[i].ManufacturingOutput / worldManufacturing * 100 
                : 0;
            TopManufacturers.Add(new EconomyItem
            {
                Rank = i + 1,
                Country = topManufacturers[i].Country.Name,
                ManufacturingPercent = $"{worldShare:F1}%"
            });
        }

        CurrencyStrengthList.Clear();
        var sortedByCurrency = countries.OrderByDescending(c => c.CurrencyStrength).Take(5).ToList();
        for (int i = 0; i < sortedByCurrency.Count; i++)
        {
            CurrencyStrengthList.Add(new CountryRankItem
            {
                Rank = i + 1,
                Name = sortedByCurrency[i].Name,
                Value = $"{sortedByCurrency[i].CurrencyStrength:F1}"
            });
        }

        LaborMarkets.Clear();
        LaborMarkets.Add(new GlobalStatisticItem { Category = "Avg Unemployment", Value = $"{countries.Average(c => c.Unemployment):F1}%" });
        LaborMarkets.Add(new GlobalStatisticItem { Category = "Avg Youth Unemployment", Value = $"{countries.Average(c => c.YouthUnemployment):F1}%" });
        LaborMarkets.Add(new GlobalStatisticItem { Category = "Avg Education Level", Value = $"{countries.Average(c => c.EducationLevel):F1}%" });
        LaborMarkets.Add(new GlobalStatisticItem { Category = "Avg Literacy Rate", Value = $"{countries.Average(c => c.LiteracyRate):F1}%" });

        KeyEconomicMetrics.Clear();
        KeyEconomicMetrics.Add(new GlobalStatisticItem { Category = "Countries with High Growth (>5%)", Value = countries.Count(c => c.EconomicGrowth > 5).ToString() });
        KeyEconomicMetrics.Add(new GlobalStatisticItem { Category = "Countries in Recession (<0%)", Value = countries.Count(c => c.EconomicGrowth < 0).ToString() });
        KeyEconomicMetrics.Add(new GlobalStatisticItem { Category = "Countries with High Inflation (>10%)", Value = countries.Count(c => c.Inflation > 10).ToString() });
        KeyEconomicMetrics.Add(new GlobalStatisticItem { Category = "Countries with High Unemployment (>15%)", Value = countries.Count(c => c.Unemployment > 15).ToString() });

        EconomyInfo = $"""
╔══════════════════════════════════════════════════════════════════════════════════════════╗
║                              WORLD ECONOMY - Turn {_simulation.CurrentTurn,-4}                                 ║
╠══════════════════════════════════════════════════════════════════════════════════════════╣

📊 GLOBAL INDICATORS
  World GDP: ${totalGdp:N0}
  Total Exports: ${totalExports:N0}
  Total Imports: ${totalImports:N0}
  Net Trade Balance: ${totalExports - totalImports:N0}
  World GDP Growth: {worldGrowth:F1}%
  Avg Inflation: {avgInflation:F1}%
  Avg Interest Rate: {avgInterest:F2}%

🏭 INDUSTRY SECTORS
  World Average:
    Agriculture: {countries.Average(c => c.AgriculturePercent):F1}%
    Manufacturing: {countries.Average(c => c.ManufacturingPercent):F1}%
    Services: {countries.Average(c => c.ServicesPercent):F1}%
    Technology: {countries.Average(c => c.TechnologyPercent):F1}%

💰 TOP GDP ECONOMIES
{string.Join("\n", topGdp.Select((c, i) => $"  {i+1}. {c.Name,-20} ${c.Gdp:N0} ({c.EconomicGrowth:+0.0;-0.0}%)"))}

📦 TOP EXPORTERS
{string.Join("\n", topExports.Select((c, i) => $"  {i+1}. {c.Name,-20} ${c.Exports:N0}"))}

 🔬 TOP TECH SECTORS
{string.Join("\n", topTech.Select((c, i) => {
    double worldShare = worldTech > 0 ? c.TechOutput / worldTech * 100 : 0;
    return $"  {i+1}. {c.Country.Name,-20} {worldShare:F1}%";
}))}

 🏭 TOP MANUFACTURERS
{string.Join("\n", topManufacturers.Select((c, i) => {
    double worldShare = worldManufacturing > 0 ? c.ManufacturingOutput / worldManufacturing * 100 : 0;
    return $"  {i+1}. {c.Country.Name,-20} {worldShare:F1}%";
}))}

💵 CURRENCY STRENGTH
{string.Join("\n", countries.OrderByDescending(c => c.CurrencyStrength).Take(5).Select((c, i) => $"  {i+1}. {c.Name,-20} {c.CurrencyStrength:F1}"))}

👥 LABOR MARKETS
  Avg Unemployment: {countries.Average(c => c.Unemployment):F1}%
  Avg Youth Unemployment: {countries.Average(c => c.YouthUnemployment):F1}%
  Avg Education Level: {countries.Average(c => c.EducationLevel):F1}
  Avg Literacy Rate: {countries.Average(c => c.LiteracyRate):F1}%

🎯 KEY ECONOMIC METRICS
  Countries with High Growth (>5%): {countries.Count(c => c.EconomicGrowth > 5)}
  Countries in Recession (<0%): {countries.Count(c => c.EconomicGrowth < 0)}
  Countries with High Inflation (>10%): {countries.Count(c => c.Inflation > 10)}
  Countries with High Unemployment (>15%): {countries.Count(c => c.Unemployment > 15)}
╚══════════════════════════════════════════════════════════════════════════════════════════╝
""";
    }

    private void UpdateWarInfo()
    {
        var wars = _simulation.GetActiveWars();
        
        if (wars.Count == 0)
        {
            WarInfo = "No active wars";
            return;
        }
        
        WarInfo = $"""
            Active Conflicts ({wars.Count})
            
            {string.Join("\n\n", wars.Select(w => {
                var attacker = _simulation.GetCountry(w.AttackerId);
                var defender = _simulation.GetCountry(w.DefenderId);
                return $"{attacker?.Name ?? w.AttackerId} vs {defender?.Name ?? w.DefenderId}\n" +
                       $"  Duration: {w.CurrentTurn - w.StartTurn} turns\n" +
                       $"  Casualties: {w.AttackerDeaths + w.DefenderDeaths:N0}";
            }))}
            """;
    }

    private string[] GetCountryNames(List<string> ids)
    {
        var names = new List<string>();
        foreach (var id in ids)
        {
            var country = _simulation.GetCountry(id);
            if (country != null)
            {
                names.Add(country.Name);
            }
        }
        return names.ToArray();
    }
}
