using System;
using System.IO;
using WorldSimApp.Models;
using WorldSimApp.Services;

namespace WorldSimApp.ViewModels;

public partial class MainWindowViewModel
{
    private void LoadData()
    {
        var jsonPath = DataPathHelper.FindFile("countries.json");
        var resourcesPath = DataPathHelper.FindFile("resources.json");
        var orgPath = DataPathHelper.FindFile("organizations.json");
        
        try
        {
            if (jsonPath != null)
            {
                _simulation.LoadCountries(jsonPath);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DEBUG: countries.json not found");
            }
            
            if (resourcesPath != null)
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG: Loading resources from {resourcesPath}");
                _simulation.LoadResources(resourcesPath);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DEBUG: resources.json not found");
            }
            
            if (orgPath != null)
            {
                _simulation.LoadOrganizations(orgPath);
            }
            
            System.Diagnostics.Debug.WriteLine($"DEBUG: After load - Countries: {_simulation.Countries.Count}, TradeRoutes: {_simulation.TradeRoutes.Count}, Resources: {_simulation.Resources.Count}");
            
            OnPropertyChanged(nameof(Countries));
            OnPropertyChanged(nameof(FilteredCountries));
            OnPropertyChanged(nameof(ActiveWars));
            OnPropertyChanged(nameof(Organizations));
            
            var tradeCount = _simulation.TradeRoutes.Count;
            var countryCount = _simulation.Countries.Count;
            WorldStatus = $"Loaded {countryCount} countries, {tradeCount} trade routes. Click 'Start'.";
            
            UpdateTradeInfo();
            UpdateEconomyInfo();
        }
        catch (Exception ex)
        {
            WorldStatus = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"DEBUG Error: {ex}");
        }
    }

    private void DebugCheckData()
    {
        var countriesCount = _simulation.Countries.Count;
        var tradeCount = _simulation.TradeRoutes.Count;
        var resourcesCount = _simulation.Resources.Count;
        System.Diagnostics.Debug.WriteLine($"DEBUG: Countries={countriesCount}, TradeRoutes={tradeCount}, Resources={resourcesCount}");
    }
}
