using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;
using Country = WorldSimApp.Models.Country;

namespace WorldSimApp.Simulation;

public class EnhancedEconomicSystem
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;

    private Dictionary<string, SupplyDemand> _marketData = new();
    private Dictionary<string, List<IndustryChain>> _industryChains = new();
    private PhillipsCurveData _phillipsCurveData = new();

    public EnhancedEconomicSystem(WorldSimulation simulation)
    {
        _simulation = simulation;
        _random = new Random();
        InitializeMarketData();
        InitializeIndustryChains();
    }

    private void InitializeMarketData()
    {
        var resources = new[] { "Oil", "NaturalGas", "Coal", "Iron", "Gold", "Food", "Technology", "Weaponry" };
        foreach (var resource in resources)
        {
            _marketData[resource] = new SupplyDemand
            {
                ResourceId = resource,
                GlobalSupply = _random.NextDouble() * 1000 + 500,
                GlobalDemand = _random.NextDouble() * 1000 + 500,
                PriceVolatility = _random.NextDouble() * 0.3 + 0.1,
                BasePrice = _random.NextDouble() * 50 + 25
            };
        }
    }

    private void InitializeIndustryChains()
    {
        _industryChains = new Dictionary<string, List<IndustryChain>>
        {
            ["Oil"] = new List<IndustryChain>
            {
                new() { Industry = "Petrochemical", InputResource = "Oil", Dependency = 0.8 },
                new() { Industry = "Transportation", InputResource = "Oil", Dependency = 0.7 },
                new() { Industry = "Manufacturing", InputResource = "Oil", Dependency = 0.4 }
            },
            ["Technology"] = new List<IndustryChain>
            {
                new() { Industry = "Electronics", InputResource = "Technology", Dependency = 0.9 },
                new() { Industry = "Communication", InputResource = "Technology", Dependency = 0.85 },
                new() { Industry = "Defense", InputResource = "Technology", Dependency = 0.5 }
            },
            ["Food"] = new List<IndustryChain>
            {
                new() { Industry = "Agriculture", InputResource = "Food", Dependency = 0.3 },
                new() { Industry = "Livestock", InputResource = "Food", Dependency = 0.6 }
            }
        };
    }

    public void SimulateEconomicChanges()
    {
        UpdateMarketDynamics();
        SimulateSupplyDemand();
        SimulateIndustryChains();
        
        foreach (var country in _simulation.Countries)
        {
            ApplyHistoricalShock(country);
            SimulateTaylorRuleMonetaryPolicy(country);
            SimulatePhillipsCurve(country);
            SimulateTradeBalance(country);
            SimulateIndustrySectors(country);
            SimulateUnemploymentDetails(country);
            SimulateExchangeRate(country);
            CalculateEconomicGrowth(country);
            SimulateInflationDynamics(country);
        }
    }
    
    private void ApplyHistoricalShock(Country country)
    {
        var shockEvents = new Dictionary<string, double>
        {
            ["oil_crisis"] = -3.0,
            ["tech_boom"] = 2.5,
            ["financial_crisis"] = -5.0,
            ["trade_war"] = -2.0,
            ["pandemic"] = -4.0,
            ["natural_disaster"] = -1.5,
            ["reform_success"] = 1.5,
            ["demographic_dividend"] = 1.0
        };
        
        double cycleMultiplier = _simulation.Settings.BusinessCycleIntensity;
        
        int yearFactor = _simulation.CurrentTurn % 20;
        if (yearFactor < 5)
        {
            if (RandomManager.Chance(0.15 * cycleMultiplier))
            {
                country.EconomicGrowth -= RandomManager.NextRange(0.5, 2.0) * cycleMultiplier;
            }
        }
        else if (yearFactor > 15)
        {
            if (RandomManager.Chance(0.1 * cycleMultiplier))
            {
                country.EconomicGrowth += RandomManager.NextRange(0.3, 1.5) * cycleMultiplier;
            }
        }
        
        double eventChance = _simulation.Settings.MajorEventFrequency * _simulation.Settings.RandomnessMultiplier;
        if (RandomManager.Chance(eventChance))
        {
            var shock = RandomManager.WeightedRandom(shockEvents);
            double impact = shockEvents[shock] * _simulation.Settings.RandomnessMultiplier;
            country.EconomicGrowth += impact;
            
            _simulation.Events.Add(new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = country.Id,
                CountryName = country.Name,
                Type = "Economic",
                Title = shock.Replace("_", " ").ToUpper(),
                Description = $"{country.Name} experienced {shock.Replace("_", " ")}",
                ImpactGdp = impact
            });
        }
    }

    private void UpdateMarketDynamics()
    {
        foreach (var market in _marketData.Values)
        {
            double supplyShock = (_random.NextDouble() - 0.5) * market.PriceVolatility;
            double demandShock = (_random.NextDouble() - 0.5) * market.PriceVolatility;
            
            market.GlobalSupply *= (1 + supplyShock);
            market.GlobalDemand *= (1 + demandShock);
            market.GlobalSupply = Math.Max(100, market.GlobalSupply);
            market.GlobalDemand = Math.Max(100, market.GlobalDemand);
            
            double imbalance = (market.GlobalDemand - market.GlobalSupply) / market.GlobalSupply;
            market.CurrentPrice = market.BasePrice * (1 + imbalance * 0.5);
            market.CurrentPrice = Math.Clamp(market.CurrentPrice, market.BasePrice * 0.5, market.BasePrice * 2);
        }
    }

    private void SimulateSupplyDemand()
    {
        foreach (var country in _simulation.Countries)
        {
            var countryResources = _simulation.CountryResources.Where(r => r.CountryId == country.Id).ToList();
            
            foreach (var resource in countryResources)
            {
                if (_marketData.TryGetValue(resource.ResourceId, out var market))
                {
                    double countryShare = resource.Surplus / market.GlobalSupply;
                    double priceEffect = (market.CurrentPrice - market.BasePrice) / market.BasePrice;
                    
                    country.ResourceDependency[resource.ResourceId] = priceEffect * countryShare;
                    
                    if (priceEffect > 0.3)
                    {
                        country.EconomicGrowth -= countryShare * 0.5;
                        country.Inflation += countryShare * priceEffect;
                    }
                    else if (priceEffect < -0.2)
                    {
                        country.EconomicGrowth += countryShare * 0.3;
                    }
                }
            }
        }
    }

    private void SimulateIndustryChains()
    {
        foreach (var country in _simulation.Countries)
        {
            foreach (var sector in new[] { "Oil", "Technology", "Food" })
            {
                if (!_industryChains.ContainsKey(sector)) continue;
                
                double sectorOutput = sector switch
                {
                    "Oil" => country.AgriculturePercent * 0.3,
                    "Technology" => country.TechnologyPercent,
                    "Food" => country.AgriculturePercent,
                    _ => 10
                };
                
                foreach (var chain in _industryChains[sector])
                {
                    double inputAvailable = _marketData.TryGetValue(chain.InputResource, out var input) 
                        ? input.CurrentPrice / input.BasePrice 
                        : 1.0;
                    
                    double efficiency = Math.Min(1.0, inputAvailable * chain.Dependency);
                    country.EconomicGrowth += sectorOutput * efficiency * 0.01;
                }
            }
        }
    }

    private void SimulateTaylorRuleMonetaryPolicy(Country country)
    {
        double inflationGap = country.Inflation - 2.0;
        double outputGap = CalculateOutputGap(country);
        
        double targetRate = country.BaseInterestRate + 1.5 * inflationGap + 0.5 * outputGap;
        targetRate = Math.Clamp(targetRate, -2, 20);
        
        double adjustmentSpeed = 0.15;
        country.InterestRate += (targetRate - country.InterestRate) * adjustmentSpeed;
        
        country.RealInterestRate = country.InterestRate - country.Inflation;
    }

    private double CalculateOutputGap(Country country)
    {
        double potentialGdp = country.PotentialGdp > 0 ? country.PotentialGdp : country.Gdp * 1.05;
        double outputGap = (country.Gdp - potentialGdp) / potentialGdp * 100;
        return Math.Clamp(outputGap, -5, 5);
    }

    private void SimulatePhillipsCurve(Country country)
    {
        _phillipsCurveData = new PhillipsCurveData
        {
            NaturalRateOfUnemployment = 5.0,
            Slope = -0.5,
            ExpectedInflation = country.Inflation * 0.7
        };

        double unemploymentGap = country.Unemployment - _phillipsCurveData.NaturalRateOfUnemployment;
        double shortRunInflation = _phillipsCurveData.ExpectedInflation + 
                                    _phillipsCurveData.Slope * unemploymentGap;
        
        country.NAIRU = _phillipsCurveData.NaturalRateOfUnemployment + 
                        (country.EducationLevel > 70 ? -1 : 0) +
                        (country.EconomicGrowth > 3 ? 0.5 : -0.5);
        
        country.NAIRU = Math.Clamp(country.NAIRU, 3, 10);
        
        double inflationPressure = shortRunInflation - country.Inflation;
        country.Inflation += inflationPressure * 0.3;
        country.Inflation = Math.Clamp(country.Inflation, 0, 25);
        
        if (country.PolicyAgenda.Contains("CentralBankIndependent"))
        {
            country.Inflation *= 0.9;
        }
    }

    private void SimulateInflationDynamics(Country country)
    {
        double demandPull = (country.EconomicGrowth - 2) * 0.2;
        double costPush = 0.0;
        
        foreach (var dependency in country.ResourceDependency)
        {
            if (dependency.Value > 0)
            {
                costPush += dependency.Value * 0.3;
            }
        }
        
        double importedInflation = 0.0;
        if (country.Imports > country.Exports)
        {
            double currentAccountDeficit = (country.Imports - country.Exports) / country.Gdp;
            importedInflation = currentAccountDeficit * 0.5;
        }
        
        double expectedInflation = country.Inflation * 0.6;
        country.Inflation = expectedInflation + demandPull + costPush + importedInflation;
        country.Inflation = Math.Clamp(country.Inflation, -2, 30);
    }

    private void SimulateTradeBalance(Country country)
    {
        var exportRoutes = _simulation.TradeRoutes
            .Where(t => t.ExporterId == country.Id && t.IsActive)
            .ToList();
        
        var importRoutes = _simulation.TradeRoutes
            .Where(t => t.ImporterId == country.Id && t.IsActive)
            .ToList();

        double exports = 0;
        foreach (var route in exportRoutes)
        {
            double routeValue = CalculateDynamicRouteValue(country, route, isExport: true);
            if (!double.IsNaN(routeValue) && !double.IsInfinity(routeValue))
                exports += routeValue;
        }
        
        double imports = 0;
        foreach (var route in importRoutes)
        {
            double routeValue = CalculateDynamicRouteValue(country, route, isExport: false);
            if (!double.IsNaN(routeValue) && !double.IsInfinity(routeValue))
                imports += routeValue;
        }

        double exportBonus = 0;
        double importCost = 0;

        foreach (var agreement in country.TradeAgreements.Where(a => a.IsActive))
        {
            exportBonus += agreement.TradeVolume * (1 - agreement.TariffRate / 100) * 0.01;
        }

        foreach (var sanction in country.SanctionedBy.Where(s => s.IsActive))
        {
            double impact = sanction.Type switch
            {
                SanctionType.TradeEmbargo => 0.3,
                SanctionType.FinancialSanctions => 0.2,
                SanctionType.ArmsEmbargo => 0.1,
                SanctionType.TravelBan => 0.05,
                SanctionType.DiplomaticSanctions => 0.02,
                _ => 0.1
            };
            importCost += imports * impact;
            
            country.Gdp *= (1 - sanction.EconomicImpact * 0.001);
        }

        country.Exports = exports * (1 + exportBonus);
        country.Imports = imports + importCost;
        country.TradeBalance = country.Exports - country.Imports;
        
        if (double.IsNaN(country.TradeBalance) || double.IsInfinity(country.TradeBalance))
            country.TradeBalance = 0;
        
        country.CurrentAccount = country.TradeBalance - (country.Gdp * 0.02);
    }

    private double CalculateDynamicRouteValue(Country country, TradeRoute route, bool isExport)
    {
        double baseValue = route.Value;
        
        if (baseValue <= 0 || double.IsNaN(baseValue) || double.IsInfinity(baseValue))
            baseValue = 1;
        
        if (route.BasePrice <= 0 || double.IsNaN(route.BasePrice))
            route.BasePrice = 50;
        
        double priceChange = (route.CurrentPrice - route.BasePrice) / route.BasePrice;
        double priceAdjustment = 1 + priceChange * 0.3;
        
        var partnerId = isExport ? route.ImporterId : route.ExporterId;
        var partner = _simulation.Countries.FirstOrDefault(c => c.Id == partnerId);
        
        double exchangeRateEffect = 1.0;
        if (partner != null)
        {
            double countryRate = country.ExchangeRate > 0 ? country.ExchangeRate : 1.0;
            double partnerRate = partner.ExchangeRate > 0 ? partner.ExchangeRate : 1.0;
            exchangeRateEffect = partnerRate / countryRate;
            if (!isExport)
            {
                exchangeRateEffect = 1 / exchangeRateEffect;
            }
        }
        
        double marketTrend = 1.0 + (_random.NextDouble() - 0.5) * route.PriceVolatility;
        
        double seasonalFactor = 1.0;
        int month = _simulation.CurrentTurn % 12;
        if (month >= 10 || month <= 2)
        {
            var energyResources = new[] { "NaturalGas", "Oil" };
            if (energyResources.Contains(route.ResourceId))
            {
                seasonalFactor = 1.2;
            }
        }
        
        double result = baseValue * priceAdjustment * exchangeRateEffect * marketTrend * seasonalFactor;
        
        if (double.IsNaN(result) || double.IsInfinity(result))
            return baseValue;
        
        return result;
    }

    private void SimulateIndustrySectors(Country country)
    {
        double developmentLevel = GetDevelopmentLevel(country);
        
        double techGrowthPotential = 0;
        if (developmentLevel > 0.7)
            techGrowthPotential = (_random.NextDouble() - 0.3) * 0.3;
        else if (developmentLevel > 0.4)
            techGrowthPotential = (_random.NextDouble() - 0.4) * 0.4;
        else
            techGrowthPotential = (_random.NextDouble() - 0.6) * 0.2;
        
        if (country.TechnologyPercent < 5 && country.EducationLevel > 60)
            techGrowthPotential += 0.2;
        
        country.TechnologyPercent += techGrowthPotential;
        
        double shift = 0;
        if (country.EconomicGrowth > 3)
        {
            shift = _random.NextDouble() * 0.4;
            country.ServicesPercent += shift * 0.7;
            country.ManufacturingPercent -= shift * 0.3;
            country.AgriculturePercent -= shift * 0.4;
        }
        else if (country.EconomicGrowth < -2)
        {
            shift = _random.NextDouble() * 0.3;
            country.AgriculturePercent += shift * 0.4;
            country.ManufacturingPercent += shift * 0.3;
            country.ServicesPercent -= shift * 0.4;
            country.TechnologyPercent -= shift * 0.3;
        }
        
        double total = country.AgriculturePercent + country.ManufacturingPercent + 
                      country.ServicesPercent + country.TechnologyPercent;
        
        if (total > 0 && Math.Abs(total - 100) > 1)
        {
            double ratio = 100 / total;
            country.AgriculturePercent *= ratio;
            country.ManufacturingPercent *= ratio;
            country.ServicesPercent *= ratio;
            country.TechnologyPercent *= ratio;
        }
        
        country.AgriculturePercent = Math.Clamp(country.AgriculturePercent, 1, 60);
        country.ManufacturingPercent = Math.Clamp(country.ManufacturingPercent, 5, 70);
        country.ServicesPercent = Math.Clamp(country.ServicesPercent, 10, 85);
        country.TechnologyPercent = Math.Clamp(country.TechnologyPercent, 1, 50);
    }

    private void SimulateUnemploymentDetails(Country country)
    {
        country.LaborForce = country.Population * (country.LaborForceParticipation / 100);

        double baseUnemployment = 5 + (country.EconomicGrowth < 0 ? Math.Abs(country.EconomicGrowth) * 2 : 0);
        double youthFactor = 1.5 + (_random.NextDouble() - 0.5) * 0.3;

        country.Unemployment = Math.Clamp(baseUnemployment + (_random.NextDouble() - 0.5) * 0.5, 0, 30);
        country.YouthUnemployment = Math.Clamp(country.Unemployment * youthFactor, 5, 60);

        if (country.EducationLevel > 70)
        {
            country.Unemployment *= 0.85;
            country.YouthUnemployment *= 0.8;
        }
    }

    private void SimulateExchangeRate(Country country)
    {
        if (country.Gdp <= 0) country.Gdp = 1;
        
        double tradeBalanceEffect = double.IsNaN(country.TradeBalance) || double.IsInfinity(country.TradeBalance) 
            ? 0 : country.TradeBalance / country.Gdp * 0.1;
        
        double interestRateEffect = (country.InterestRate - 2) * 0.05;
        double inflationEffect = -(country.Inflation / 100);

        double targetRate = 1.0 + tradeBalanceEffect + interestRateEffect + inflationEffect;
        targetRate = Math.Clamp(targetRate, 0.5, 2.0);

        if (country.ExchangeRate <= 0 || double.IsNaN(country.ExchangeRate))
            country.ExchangeRate = 1.0;
        
        country.ExchangeRate += (targetRate - country.ExchangeRate) * 0.05;
        country.CurrencyStrength = country.ExchangeRate * 100;
    }

    private void CalculateEconomicGrowth(Country country)
    {
        if (country.PotentialGdp == 0) country.PotentialGdp = country.Gdp;
        if (country.CapitalStock == 0) country.CapitalStock = country.Gdp * 2;
        if (country.GdpPerCapita == 0) country.GdpPerCapita = country.Population > 0 ? country.Gdp / (country.Population / 1000000) : 0;

        double outputGap = (country.Gdp - country.PotentialGdp) / country.PotentialGdp;
        outputGap = Math.Clamp(outputGap, -0.3, 0.3);

        UpdateCapacityUtilization(country, outputGap);
        
        double potentialGrowth = CalculatePotentialGrowth(country);
        double actualGrowth = CalculateActualGrowth(country);
        double blendedGrowth = BlendGrowth(country, potentialGrowth, actualGrowth, outputGap);
        
        ApplyGrowth(country, blendedGrowth);
        
        UpdateCapitalStock(country);
        
        UpdatePerCapitaGDP(country);
        
        UpdateCountrySpecificHappiness(country);
        
        NormalizeIndustrySectors(country);
    }

    private double CalculatePotentialGrowth(Country country)
    {
        double developmentLevel = GetDevelopmentLevel(country);
        
        double catchUpBonus = 0;
        if (developmentLevel < 0.4)
        {
            catchUpBonus = (0.4 - developmentLevel) * 0.5;
        }
        else if (developmentLevel > 0.7)
        {
            double maturityPenalty = (developmentLevel - 0.7) * 0.5;
            catchUpBonus = -maturityPenalty;
        }
        
        double capitalShare = 0.35;
        double laborShare = 0.65;
        
        double capitalGrowth = (country.InvestmentRate / 100 - country.DepreciationRate) * capitalShare;
        
        double laborGrowth = country.LaborForceGrowth * laborShare;
        
        double tfpGrowth = country.TotalFactorProductivity * 0.015;
        
        double educationImpact = (country.EducationLevel - 50) * 0.001;
        
        double techImpact = country.TechnologyPercent * 0.002;
        
        double potentialGrowth = capitalGrowth + laborGrowth + tfpGrowth + educationImpact + techImpact + catchUpBonus;
        
        double agePenalty = 0;
        if (country.MedianAge > 40)
        {
            agePenalty = (country.MedianAge - 40) * 0.001;
        }
        potentialGrowth -= agePenalty;
        
        return Math.Clamp(potentialGrowth, 0.005, 0.06);
    }

    private double CalculateActualGrowth(Country country)
    {
        double previousGrowth = Math.Max(0, country.EconomicGrowth) / 100;
        
        double consumptionGrowth = previousGrowth * 0.25;
        
        double accelerator = Math.Max(0, previousGrowth) * 0.3;
        double investmentGrowth = country.InvestmentRate * accelerator / 10000;
        
        double governmentEffect = (country.GovernmentSpending - 15) * 0.0008;
        
        double tradeBalanceRatio = country.TradeBalance / Math.Max(1, country.Gdp);
        double netExportEffect = tradeBalanceRatio * 0.03;
        
        double allyTradeEffect = 0;
        if (_simulation.TradeRoutes.Count > 0)
        {
            foreach (var allyId in country.Allies)
            {
                var ally = _simulation.Countries.FirstOrDefault(c => c.Id == allyId);
                if (ally != null)
                {
                    allyTradeEffect += ally.EconomicGrowth * 0.001;
                }
            }
        }
        
        double resourceSelfSufficiencyEffect = 0;
        var countryResources = _simulation.CountryResources.Where(r => r.CountryId == country.Id).ToList();
        foreach (var resource in countryResources)
        {
            var res = _simulation.Resources.FirstOrDefault(r => r.Id == resource.ResourceId);
            if (res != null && resource.Consumption > 0)
            {
                double selfSufficiency = resource.Production / resource.Consumption;
                
                if (selfSufficiency < 0.5)
                {
                    resourceSelfSufficiencyEffect -= (0.5 - selfSufficiency) * res.Importance * 0.02;
                }
                else if (selfSufficiency > 1.2)
                {
                    resourceSelfSufficiencyEffect += (selfSufficiency - 1) * res.Importance * 0.01;
                }
            }
        }
        
        double organizationBonus = 0;
        foreach (var org in _simulation.Organizations.Where(o => o.MemberIds.Contains(country.Id)))
        {
            organizationBonus += org.EffectTrade * 0.001;
            organizationBonus += org.EffectStability * 0.0001;
        }
        
        double crisisPenalty = 0;
        if (country.Inflation > 10) crisisPenalty -= 0.02;
        if (country.Unemployment > 15) crisisPenalty -= (country.Unemployment - 15) * 0.002;
        if (country.Stability < 40) crisisPenalty -= (40 - country.Stability) * 0.001;
        if (country.CrimeRate > 15) crisisPenalty -= 0.01;
        
        double opportunityBonus = 0;
        if (country.EducationLevel > 70 && country.TechnologyPercent < 20)
            opportunityBonus += 0.01;
        if (country.NetMigration > 2)
            opportunityBonus += 0.005;
        
        double developmentLevel = GetDevelopmentLevel(country);
        double developmentBonus = 0;
        if (developmentLevel > 0.7)
        {
            developmentBonus += country.TechnologyPercent * 0.0005;
            developmentBonus -= country.AgingIndex * 0.005;
            developmentBonus -= country.PensionPressure > 30 ? 0.005 : 0;
        }
        else if (developmentLevel < 0.4)
        {
            developmentBonus += country.FertilityRate * 0.05;
            developmentBonus += country.InfrastructureQuality * 0.0002;
            developmentBonus -= country.CorruptionIndex * 0.0002;
        }
        
        double sectorBonus = 0;
        if (country.TechnologyPercent > 30)
            sectorBonus += 0.015;
        else if (country.TechnologyPercent > 20)
            sectorBonus += 0.008;
        
        if (country.ServicesPercent > 60)
            sectorBonus += 0.005;
        
        if (country.ManufacturingPercent > 30 && country.ManufacturingPercent < 50)
            sectorBonus += 0.003;
        
        if (country.AgriculturePercent > 30)
            sectorBonus -= 0.005;
        
        double stabilityEffect = (country.Stability - 50) * 0.00015;
        
        double inflationPenalty = Math.Max(0, country.Inflation - 3) * 0.0008;
        
        double corruptionPenalty = country.CorruptionIndex > 40 ? (country.CorruptionIndex - 40) * 0.00015 : 0;
        
        double infrastructurePenalty = country.InfrastructureQuality < 40 ? (40 - country.InfrastructureQuality) * 0.0002 : 0;
        
        double actualGrowth = consumptionGrowth + investmentGrowth + governmentEffect + 
                            netExportEffect + allyTradeEffect + resourceSelfSufficiencyEffect + 
                            organizationBonus + developmentBonus + sectorBonus + stabilityEffect + opportunityBonus + 
                            crisisPenalty - inflationPenalty - corruptionPenalty - infrastructurePenalty;
        
        return actualGrowth;
    }

    private double BlendGrowth(Country country, double potentialGrowth, double actualGrowth, double outputGap)
    {
        double developmentLevel = GetDevelopmentLevel(country);
        
        double baseWeight;
        if (developmentLevel < 0.4)
        {
            baseWeight = 0.25;
        }
        else if (developmentLevel < 0.7)
        {
            baseWeight = 0.4;
        }
        else
        {
            baseWeight = 0.65;
        }
        
        double outputGapAdjustment = outputGap * 5;
        
        double adjustedWeight = Math.Clamp(baseWeight + outputGapAdjustment, 0.15, 0.8);
        
        double blendedGrowth = potentialGrowth * adjustedWeight + actualGrowth * (1 - adjustedWeight);
        
        blendedGrowth = Math.Clamp(blendedGrowth, -0.05, 0.07);
        
        return blendedGrowth;
    }

    private double GetDevelopmentLevel(Country country)
    {
        double gdpScore = Math.Min(country.GdpPerCapita / 50000, 1.0);
        double educationScore = country.EducationLevel / 100;
        double healthScore = country.HealthcareLevel / 100;
        
        return (gdpScore * 0.4 + educationScore * 0.3 + healthScore * 0.3);
    }

    private void UpdateCountrySpecificHappiness(Country country)
    {
        double developmentLevel = GetDevelopmentLevel(country);
        
        double baseHappiness = 45 + developmentLevel * 20;
        
        double gdpEffect = (country.GdpPerCapita - 25000) / 25000 * 8;
        gdpEffect = Math.Clamp(gdpEffect, -8, 10);
        
        double stabilityEffect = (country.Stability - 50) / 50 * 15;
        
        double unemploymentEffect = -(country.Unemployment - 6) * 0.6;
        
        double inflationEffect = -(country.Inflation - 3) * 0.4;
        
        double socialEffect = 0;
        if (country.IncomeInequality > 45)
            socialEffect -= (country.IncomeInequality - 45) * 0.15;
        if (country.SocialMobility > 35)
            socialEffect += 3;
        
        double negativePressure = 0;
        if (country.Happiness > 70)
            negativePressure = -(country.Happiness - 70) * 0.1;
        if (country.AgingIndex > 0.8)
            negativePressure -= (country.AgingIndex - 0.8) * 5;
        
        double change = gdpEffect + stabilityEffect + unemploymentEffect + inflationEffect + socialEffect + negativePressure;
        change += (_random.NextDouble() - 0.5) * 1.5;
        
        double targetHappiness = Math.Clamp(baseHappiness + change, 5, 85);
        
        country.Happiness += (targetHappiness - country.Happiness) * 0.2;
        country.Happiness = Math.Clamp(country.Happiness, 5, 90);
    }

    private void NormalizeIndustrySectors(Country country)
    {
        double total = country.AgriculturePercent + country.ManufacturingPercent + 
                      country.ServicesPercent + country.TechnologyPercent;
        
        if (Math.Abs(total - 100) > 0.01)
        {
            country.AgriculturePercent = country.AgriculturePercent / total * 100;
            country.ManufacturingPercent = country.ManufacturingPercent / total * 100;
            country.ServicesPercent = country.ServicesPercent / total * 100;
            country.TechnologyPercent = country.TechnologyPercent / total * 100;
        }
        
        country.AgriculturePercent = Math.Clamp(country.AgriculturePercent, 1, 50);
        country.ManufacturingPercent = Math.Clamp(country.ManufacturingPercent, 5, 60);
        country.ServicesPercent = Math.Clamp(country.ServicesPercent, 10, 80);
        country.TechnologyPercent = Math.Clamp(country.TechnologyPercent, 1, 30);
        
        total = country.AgriculturePercent + country.ManufacturingPercent + 
                country.ServicesPercent + country.TechnologyPercent;
        
        if (Math.Abs(total - 100) > 0.01)
        {
            double ratio = 100 / total;
            country.AgriculturePercent *= ratio;
            country.ManufacturingPercent *= ratio;
            country.ServicesPercent *= ratio;
            country.TechnologyPercent *= ratio;
        }
    }

    private void ApplyGrowth(Country country, double growth)
    {
        double gdpIncrease = country.Gdp * growth;
        
        country.Gdp += gdpIncrease;
        
        country.EconomicGrowth = growth * 100;
        
        country.PotentialGdp *= (1 + growth * 0.7);
        
        UpdateLaborForce(country);
    }

    private void UpdateCapacityUtilization(Country country, double outputGap)
    {
        double targetUtilization = 80 + outputGap * 50;
        targetUtilization = Math.Clamp(targetUtilization, 60, 100);
        
        country.CapacityUtilization += (targetUtilization - country.CapacityUtilization) * 0.3;
    }

    private void UpdateCapitalStock(Country country)
    {
        double grossInvestment = country.Gdp * country.InvestmentRate / 100;
        
        double depreciation = country.CapitalStock * country.DepreciationRate;
        
        country.CapitalStock += grossInvestment - depreciation;
        
        country.CapitalStock = Math.Max(country.CapitalStock, country.Gdp);
    }

    private void UpdatePerCapitaGDP(Country country)
    {
        double oldPerCapita = country.GdpPerCapita;
        
        country.GdpPerCapita = country.Population > 0 ? country.Gdp / (country.Population / 1000000) : 0;
        
        country.GdpPerCapitaGrowth = country.GdpPerCapita - oldPerCapita;
    }

    private void UpdateLaborForce(Country country)
    {
        double laborForceGrowth = country.LaborForceParticipation * 0.001;
        
        if (country.MedianAge > 40)
            laborForceGrowth *= 0.9;
        
        country.LaborForceGrowth = laborForceGrowth;
        
        country.LaborForce = country.Population * (country.LaborForceParticipation / 100);
    }
}

public class SupplyDemand
{
    public string ResourceId { get; set; } = string.Empty;
    public double GlobalSupply { get; set; }
    public double GlobalDemand { get; set; }
    public double CurrentPrice { get; set; }
    public double BasePrice { get; set; }
    public double PriceVolatility { get; set; }
}

public class IndustryChain
{
    public string Industry { get; set; } = string.Empty;
    public string InputResource { get; set; } = string.Empty;
    public double Dependency { get; set; }
}

public class PhillipsCurveData
{
    public double NaturalRateOfUnemployment { get; set; }
    public double Slope { get; set; }
    public double ExpectedInflation { get; set; }
}
