using System;
using System.Collections.Generic;
using System.Linq;
using WorldSimApp.Models;

namespace WorldSimApp.Simulation;

public class TradeSimulator
{
    private readonly Random _random;
    private readonly WorldSimulation _simulation;
    private readonly GameSettings _settings;
    private readonly Dictionary<string, double> _resourceBasePrices = new()
    {
        ["Oil"] = 2000.0,
        ["NaturalGas"] = 1200.0,
        ["Coal"] = 800.0,
        ["Iron"] = 1000.0,
        ["Gold"] = 50000.0,
        ["Food"] = 500.0,
        ["Technology"] = 15000.0,
        ["Weaponry"] = 8000.0,
        ["Electronics"] = 5000.0,
        ["Machinery"] = 4000.0,
        ["Textiles"] = 400.0,
        ["Chemicals"] = 2000.0
    };

    private const double TradeScaleFactor = 0.001;

    public TradeSimulator(WorldSimulation simulation, GameSettings settings)
    {
        _simulation = simulation;
        _settings = settings;
        _random = new Random();
    }

    public void SimulateTrade()
    {
        if (!_settings.TradeEnabled) return;
        
        UpdateExistingRoutes();
        CreateNewTradeRoutes();
    }

    private void UpdateExistingRoutes()
    {
        bool shouldCalculate = _simulation.CurrentTurn >= 1;
        
        foreach (var route in _simulation.TradeRoutes.Where(t => t.IsActive).ToList())
        {
            if (shouldCalculate && route.Value <= 0)
            {
                RecalculateRouteFromScratch(route);
            }
            else if (shouldCalculate)
            {
                UpdateRouteValue(route);
            }
        }
    }

    private void RecalculateRouteFromScratch(TradeRoute route)
    {
        var exporter = _simulation.Countries.FirstOrDefault(c => c.Id == route.ExporterId);
        var importer = _simulation.Countries.FirstOrDefault(c => c.Id == route.ImporterId);
        if (exporter == null || importer == null) return;

        if (route.BasePrice <= 0)
        {
            route.BasePrice = _resourceBasePrices.GetValueOrDefault(route.ResourceId, 500.0);
        }
        
        if (route.Distance <= 0)
        {
            route.Distance = CalculateDistance(exporter, importer);
        }
        
        if (route.TransportCost <= 0)
        {
            route.TransportCost = CalculateTransportCost(route.Distance, route.ResourceId, exporter, importer);
        }

        var exporterResource = _simulation.CountryResources.FirstOrDefault(r => 
            r.CountryId == exporter.Id && r.ResourceId == route.ResourceId);
        var importerResource = _simulation.CountryResources.FirstOrDefault(r => 
            r.CountryId == importer.Id && r.ResourceId == route.ResourceId);
        
        double supply = exporterResource?.Surplus > 0 ? exporterResource.Surplus : exporter.Gdp / 1e10;
        double demand = importerResource != null && importerResource.Surplus < 0 
            ? -importerResource.Surplus 
            : importerResource != null ? importerResource.Consumption * 0.8 : importer.Gdp / 1e10;
        
        double gravityTrade = CalculateGravityModel(exporter, importer, route.Distance);
        double newAmount = Math.Min(supply, demand) * 0.5;
        newAmount = Math.Max(newAmount, gravityTrade * 0.1);
        newAmount = Math.Max(newAmount, 100);
        
        route.Amount = newAmount;
        
        double supplyDemandFactor = CalculateSupplyDemandFactor(supply, demand);
        double priceDynamic = CalculatePriceDynamic(route, supplyDemandFactor);
        double politicalRisk = CalculatePoliticalRisk(exporter, importer);
        double tariffEffect = CalculateTariffEffect(exporter, importer, route);
        
        route.CurrentPrice = route.BasePrice * priceDynamic;
        
        // double tradeScaleFactor = 10;
        route.Value = route.Amount * route.CurrentPrice * (1 - route.TransportCost) * tariffEffect * TradeScaleFactor;
        
        if (exporter.Allies.Contains(importer.Id))
        {
            route.Value *= 1.1;
        }
        
        if (exporter.Enemies.Contains(importer.Id))
        {
            route.Value *= 0.5;
        }
        
        var importerSanctions = importer.SanctionedBy.Where(s => s.ImposingCountryId == exporter.Id && s.IsActive).ToList();
        foreach (var sanction in importerSanctions)
        {
            if (sanction.Type == SanctionType.TradeEmbargo)
                route.Value *= 0.3;
            else if (sanction.Type == SanctionType.FinancialSanctions)
                route.Value *= 0.7;
        }
    }

    private void UpdateRouteValue(TradeRoute route)
    {
        var exporter = _simulation.Countries.FirstOrDefault(c => c.Id == route.ExporterId);
        var importer = _simulation.Countries.FirstOrDefault(c => c.Id == route.ImporterId);
        if (exporter == null || importer == null) return;

        if (route.BasePrice <= 0)
        {
            route.BasePrice = _resourceBasePrices.GetValueOrDefault(route.ResourceId, 500.0);
        }
        
        if (route.TransportCost <= 0)
        {
            route.TransportCost = 0.1;
        }
        
        if (route.Distance <= 0)
        {
            route.Distance = CalculateDistance(exporter, importer);
        }

        var exporterResource = _simulation.CountryResources.FirstOrDefault(r => r.CountryId == exporter.Id && r.ResourceId == route.ResourceId);
        var importerResource = _simulation.CountryResources.FirstOrDefault(r => r.CountryId == importer.Id && r.ResourceId == route.ResourceId);
        
        double supply = exporterResource?.Surplus > 0 ? exporterResource.Surplus : exporter.Gdp / 1e10;
        double demand = importerResource != null && importerResource.Surplus < 0 
            ? -importerResource.Surplus 
            : importerResource != null ? importerResource.Consumption * 0.8 : importer.Gdp / 1e10;
        
        double gravityTrade = CalculateGravityModel(exporter, importer, route.Distance);
        double newAmount = Math.Min(supply, demand) * 0.5;
        newAmount = Math.Max(newAmount, gravityTrade * 0.1);
        newAmount = Math.Max(newAmount, 100);
        
        route.Amount = newAmount;
        
        double supplyDemandFactor = CalculateSupplyDemandFactor(supply, demand);
        double priceDynamic = CalculatePriceDynamic(route, supplyDemandFactor);
        double politicalRisk = CalculatePoliticalRisk(exporter, importer);
        double tariffEffect = CalculateTariffEffect(exporter, importer, route);
        
        route.CurrentPrice = route.BasePrice * priceDynamic;
        
        // double tradeScaleFactor = 10;
        route.Value = route.Amount * route.CurrentPrice * (1 - route.TransportCost) * tariffEffect * TradeScaleFactor;
        
        if (exporter.Allies.Contains(importer.Id))
        {
            route.Value *= 1.1;
        }
        
        if (exporter.Enemies.Contains(importer.Id))
        {
            route.Value *= 0.5;
            if (_random.NextDouble() < 0.1)
            {
                route.IsActive = false;
            }
        }
        
        var tradeSanction = importer.SanctionedBy.FirstOrDefault(s => 
            s.ImposingCountryId == exporter.Id && s.Type == SanctionType.TradeEmbargo);
        if (tradeSanction != null)
        {
            route.Value *= (1 - tradeSanction.EconomicImpact * 0.1);
            if (_random.NextDouble() < 0.2)
            {
                route.IsActive = false;
            }
        }
    }

    private double CalculateSupplyDemandFactor(double supply, double demand)
    {
        if (supply + demand <= 0) return 1.0;
        return 1.0 + (demand - supply * 0.5) / (supply + demand + 100) * 0.5;
    }

    private double CalculatePriceDynamic(TradeRoute route, double supplyDemandFactor)
    {
        double baseDynamic = supplyDemandFactor;
        double volatilityEffect = 1 + (_random.NextDouble() - 0.5) * route.PriceVolatility;
        double timeDecay = Math.Max(0.8, 1.0 - (_simulation.CurrentTurn - route.TurnEstablished) * 0.005);
        return baseDynamic * volatilityEffect * timeDecay;
    }

    private double CalculatePoliticalRisk(Country exporter, Country importer)
    {
        double risk = 0.0;
        
        if (exporter.Stability < 40) risk += 0.1;
        if (importer.Stability < 40) risk += 0.1;
        
        if (exporter.Enemies.Contains(importer.Id)) risk += 0.3;
        
        if (exporter.DiplomaticRelations.TryGetValue(importer.Id, out var relation))
        {
            if (relation < -50) risk += 0.2;
            else if (relation > 50) risk -= 0.1;
        }
        
        return Math.Clamp(risk, 0, 0.5);
    }

    private double CalculateTariffEffect(Country exporter, Country importer, TradeRoute route)
    {
        double effect = 1.0;
        
        foreach (var agreement in exporter.TradeAgreements.Where(a => a.IsActive && a.PartnerId == importer.Id))
        {
            effect *= (1 - agreement.TariffRate / 100);
        }
        
        if (exporter.SanctionedBy.Any(s => s.ImposingCountryId == importer.Id && s.IsActive && s.Type == SanctionType.TradeEmbargo))
        {
            effect *= 0.3;
        }
        
        effect *= (1 - route.TariffRate / 100);
        
        return Math.Clamp(effect, 0.1, 1.5);
    }

    private double CalculateGravityModel(Country exporter, Country importer, double distance)
    {
        double gdp1 = exporter.Gdp;
        double gdp2 = importer.Gdp;
        
        double distanceFactor = Math.Pow(500 / Math.Max(distance, 100), 0.8);
        
        double tradeOpenness = 0.25;
        double totalExports = exporter.Exports > 0 ? exporter.Exports : gdp1 * 0.12;
        double totalImports = importer.Imports > 0 ? importer.Imports : gdp2 * 0.12;
        double totalGdp = gdp1 + gdp2;
        if (totalGdp > 0)
        {
            tradeOpenness = (totalExports + totalImports) / totalGdp;
            tradeOpenness = Math.Clamp(tradeOpenness, 0.08, 0.4);
        }
        
        double expectedTrade = (gdp1 + gdp2) * tradeOpenness * 0.08;
        
        return expectedTrade * distanceFactor * 0.1;
    }

    private double CalculateDistance(Country exporter, Country importer)
    {
        double distance = _random.NextDouble() * 5000 + 500;
        
        if (exporter.Region == importer.Region)
        {
            distance *= 0.3;
        }
        
        return distance;
    }

    private void CreateNewTradeRoutes()
    {
        if (_random.NextDouble() >= 0.1 * _settings.EventFrequency) return;
        
        var exporter = _simulation.Countries[_random.Next(_simulation.Countries.Count)];
        var importer = _simulation.Countries.FirstOrDefault(c => 
            c.Id != exporter.Id && 
            !exporter.Enemies.Contains(c.Id) &&
            !exporter.SanctionedBy.Any(s => s.TargetCountryId == c.Id && s.IsActive));
        
        if (importer == null) return;
        
        var exporterResource = _simulation.CountryResources
            .Where(r => r.CountryId == exporter.Id && r.Surplus > 0)
            .OrderByDescending(r => r.Surplus * r.Price)
            .FirstOrDefault();
        
        if (exporterResource == null) return;
        
        var existingRoute = _simulation.TradeRoutes.FirstOrDefault(t => 
            t.ExporterId == exporter.Id && 
            t.ImporterId == importer.Id && 
            t.ResourceId == exporterResource.ResourceId &&
            t.IsActive);
        
        if (existingRoute != null) return;
        
        double distance = CalculateDistance(exporter, importer);
        double gravityTrade = CalculateGravityModel(exporter, importer, distance);
        
        double supply = exporterResource.Surplus > 0 ? exporterResource.Surplus : exporter.Gdp / 1e10;
        var importerRes = _simulation.CountryResources.FirstOrDefault(r => 
            r.CountryId == importer.Id && 
            r.ResourceId == exporterResource.ResourceId);
        double demand = importerRes != null && importerRes.Surplus < 0 
            ? -importerRes.Surplus 
            : importerRes != null ? importerRes.Consumption * 0.8 : importer.Gdp / 1e10;
        
        double matchedAmount = Math.Min(supply, demand) * 0.5;
        matchedAmount = Math.Max(matchedAmount, gravityTrade * 0.1);
        matchedAmount = Math.Max(matchedAmount, 100);
        
        double basePrice = _resourceBasePrices.GetValueOrDefault(exporterResource.ResourceId, exporterResource.Price);
        double supplyDemandFactor = CalculateSupplyDemandFactor(supply, demand);
        double marketPrice = basePrice * supplyDemandFactor;
        
        double transportCost = CalculateTransportCost(distance, exporterResource.ResourceId, exporter, importer);
        double tariffRate = GetBaseTariffRate(exporter, importer);
        
        // double tradeScaleFactor = 10;
        double tradeValue = matchedAmount * marketPrice * (1 - transportCost) * (1 - tariffRate / 100) * TradeScaleFactor;
        
        var newRoute = new TradeRoute
        {
            ExporterId = exporter.Id,
            ImporterId = importer.Id,
            ResourceId = exporterResource.ResourceId,
            Amount = matchedAmount,
            Value = tradeValue,
            TurnEstablished = _simulation.CurrentTurn,
            Distance = distance,
            BasePrice = basePrice,
            CurrentPrice = marketPrice,
            PriceVolatility = _random.NextDouble() * 0.2 + 0.1,
            TransportCost = transportCost,
            TariffRate = tariffRate,
            IsActive = true
        };
        
        _simulation.TradeRoutes.Add(newRoute);
        
        _simulation.Events.Add(new SimulationEvent
        {
            Turn = _simulation.CurrentTurn,
            CountryId = exporter.Id,
            CountryName = exporter.Name,
            Type = "Economic",
            Title = "New Trade Route",
            Description = $"{exporter.Name} established a trade route with {importer.Name} for {exporterResource.ResourceId}. " +
                         $"Volume: {matchedAmount:F1}, Value: ${tradeValue:F1}M",
            ImpactGdp = 0.5 + tradeValue / 1000
        });
    }

    private double CalculateTransportCost(double distance, string resourceId, Country exporter, Country importer)
    {
        double baseTransportRate = 0.05;
        
        var highValueResources = new[] { "Gold", "Technology", "Weaponry", "Electronics" };
        if (highValueResources.Contains(resourceId))
        {
            baseTransportRate *= 0.5;
        }
        
        var bulkyResources = new[] { "Coal", "Iron", "Food" };
        if (bulkyResources.Contains(resourceId))
        {
            baseTransportRate *= 1.5;
        }
        
        if (exporter.Region == importer.Region)
        {
            baseTransportRate *= 0.5;
        }
        
        double distanceEffect = distance / 5000.0;
        
        double politicalRisk = CalculatePoliticalRisk(exporter, importer);
        
        double fuelFactor = 1.0 + (_random.NextDouble() - 0.5) * 0.2;
        
        return Math.Clamp(baseTransportRate * distanceEffect * (1 + politicalRisk) * fuelFactor, 0.02, 0.4);
    }

    private double GetBaseTariffRate(Country exporter, Country importer)
    {
        if (exporter.Allies.Contains(importer.Id))
        {
            return 0.0;
        }
        
        if (exporter.TradeAgreements.Any(a => a.PartnerId == importer.Id && a.IsActive))
        {
            return 2.0;
        }
        
        if (exporter.CultureGroup == importer.CultureGroup)
        {
            return 3.0;
        }
        
        return _random.NextDouble() * 8 + 5;
    }
}
