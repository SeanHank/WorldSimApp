using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorldSimApp.Models;
using WorldSimApp.Services;

namespace WorldSimApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly WorldSimulation _simulation = new();
    private CancellationTokenSource? _simulationCts;

    [ObservableProperty]
    private string _worldStatus = "Click 'Start Simulation' to begin";

    [ObservableProperty]
    private string _selectedCountryName = "";

    [ObservableProperty]
    private string _selectedCountryInfo = "Select a country to view details";

    [ObservableProperty]
    private Country? _selectedCountry;

    [ObservableProperty]
    private bool _isSimulationRunning;

    [ObservableProperty]
    private int _simulationSpeed = 1000;

    [ObservableProperty]
    private string _selectedRegion = "All";

    [ObservableProperty]
    private bool _eventSortAscending = false;

    [ObservableProperty]
    private string _sortBy = "GDP";

    [ObservableProperty]
    private bool _isDarkTheme = true;

    [ObservableProperty]
    private string _difficulty = "Normal";

    [ObservableProperty]
    private string? _playerCountryId;

    [ObservableProperty]
    private string _playerCountryDisplay = "";

    [ObservableProperty]
    private DecisionEvent? _pendingDecision;

    [ObservableProperty]
    private bool _hasPendingDecision;

    [ObservableProperty]
    private Country? _compareCountry1;

    [ObservableProperty]
    private Country? _compareCountry2;

    [ObservableProperty]
    private string _compareInfo = "Select two countries to compare";

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private string _chartData = "";

    [ObservableProperty]
    private string _tradeInfo = "";

    [ObservableProperty]
    private string _warInfo = "";

    [ObservableProperty]
    private string _economyInfo = "";

    [ObservableProperty]
    private Country? _actionTargetCountry;

    [ObservableProperty]
    private string _playerActionResult = "";

    [ObservableProperty]
    private string _selectedPolicy = "None";

    public ObservableCollection<Country> Countries => new(_simulation.Countries);
    public ObservableCollection<Country> FilteredCountries => new(GetFilteredCountries());
    public ObservableCollection<SimulationEvent> Events => new(GetSortedEvents());
    public ObservableCollection<War> ActiveWars => new(_simulation.GetActiveWars());
    public ObservableCollection<InternationalOrganization> Organizations => new(_simulation.Organizations);
    public ObservableCollection<string> Regions { get; } = new() { "All", "North America", "South America", "Europe", "Asia", "Middle East", "Africa", "Oceania" };
    public ObservableCollection<string> SortOptions { get; } = new() { "GDP", "Population", "Military", "Stability", "Happiness" };
    public ObservableCollection<string> DifficultyOptions { get; } = new() { "Easy", "Normal", "Hard" };
    public ObservableCollection<string> PolicyOptions { get; } = new() { "None", "Tax Increase", "Tax Cut", "Military Buildup", "Demilitarize", "Welfare", "Education" };

    public ObservableCollection<CountryRankItem> TopGdpCountries { get; } = new();
    public ObservableCollection<CountryRankItem> TopMilitaryCountries { get; } = new();
    public ObservableCollection<CountryRankItem> TopHappinessCountries { get; } = new();
    public ObservableCollection<CountryRankItem> TopTechCountries { get; } = new();
    public ObservableCollection<WorldOverviewItem> WorldOverview { get; } = new();
    public ObservableCollection<GlobalStatisticItem> IndustryBreakdown { get; } = new();
    public ObservableCollection<GlobalStatisticItem> PopulationTrends { get; } = new();
    public ObservableCollection<ConflictItem> Conflicts { get; } = new();
    public ObservableCollection<string> PoliticalSpectrum { get; } = new();
    public ObservableCollection<string> DiplomaticStatus { get; } = new();

    public ObservableCollection<EconomyItem> TopEconomies { get; } = new();
    public ObservableCollection<EconomyItem> TopExporters { get; } = new();
    public ObservableCollection<EconomyItem> TopTechSectors { get; } = new();
    public ObservableCollection<EconomyItem> TopManufacturers { get; } = new();
    public ObservableCollection<CountryRankItem> CurrencyStrengthList { get; } = new();
    public ObservableCollection<GlobalStatisticItem> LaborMarkets { get; } = new();
    public ObservableCollection<GlobalStatisticItem> KeyEconomicMetrics { get; } = new();
    public ObservableCollection<GlobalStatisticItem> GlobalIndicators { get; } = new();
    public ObservableCollection<GlobalStatisticItem> IndustrySectors { get; } = new();

    public ObservableCollection<CompareItem> CompareItems { get; } = new();

    public ObservableCollection<CountryDetailItem> CountryDetails { get; } = new();

    public ObservableCollection<TradeRouteItem> TradeRoutes { get; } = new();
    public ObservableCollection<CountryRankItem> TopTraders { get; } = new();

    [ObservableProperty]
    private int _currentTurn;

    public MainWindowViewModel()
    {
        ThemeService.Instance.SetTheme(IsDarkTheme);
        LoadData();
    }

    private List<Country> GetFilteredCountries()
    {
        var list = _simulation.Countries.ToList();
        
        if (SelectedRegion != "All")
        {
            list = list.Where(c => c.Region == SelectedRegion).ToList();
        }
        
        list = SortBy switch
        {
            "GDP" => list.OrderByDescending(c => c.Gdp).ToList(),
            "Population" => list.OrderByDescending(c => c.Population).ToList(),
            "Military" => list.OrderByDescending(c => c.MilitaryPower).ToList(),
            "Stability" => list.OrderByDescending(c => c.Stability).ToList(),
            "Happiness" => list.OrderByDescending(c => c.Happiness).ToList(),
            _ => list
        };
        
        return list;
    }

    [RelayCommand]
    private void StartSimulation()
    {
        if (IsSimulationRunning) return;
        
        IsSimulationRunning = true;
        _simulationCts = new CancellationTokenSource();
        
        Task.Run(async () =>
        {
            while (!_simulationCts.Token.IsCancellationRequested)
            {
                await Task.Delay(SimulationSpeed, _simulationCts.Token);
                
                Dispatcher.UIThread.Post(() =>
                {
                    _simulation.NextTurn();
                    WorldStatus = _simulation.GetWorldStatus();
                    OnPropertyChanged(nameof(Countries));
                    OnPropertyChanged(nameof(FilteredCountries));
                    OnPropertyChanged(nameof(Events));
                    OnPropertyChanged(nameof(ActiveWars));
                    
                    CheckPendingDecision();
                    
                    if (SelectedCountry != null)
                    {
                        UpdateSelectedCountryInfo();
                    }
                    
                    UpdateChartData();
                    UpdateTradeInfo();
                    UpdateWarInfo();
                    UpdateEconomyInfo();
                });
            }
        });
    }

    [RelayCommand]
    private void StopSimulation()
    {
        _simulationCts?.Cancel();
        IsSimulationRunning = false;
        WorldStatus = "Simulation paused";
    }

    [RelayCommand]
    private void ResetSimulation()
    {
        _simulationCts?.Cancel();
        IsSimulationRunning = false;
        _simulation.Settings = Difficulty switch
        {
            "Easy" => GameSettings.Easy(),
            "Hard" => GameSettings.Hard(),
            _ => GameSettings.Normal()
        };
        _simulation.Initialize();
        
        OnPropertyChanged(nameof(Countries));
        OnPropertyChanged(nameof(FilteredCountries));
        OnPropertyChanged(nameof(Events));
        OnPropertyChanged(nameof(ActiveWars));
        OnPropertyChanged(nameof(Organizations));
        
        WorldStatus = "Simulation reset. Click 'Start' to begin.";
        SelectedCountryName = "";
        SelectedCountryInfo = "Select a country to view details";
        CountryDetails.Clear();
        CompareCountry1 = null;
        CompareCountry2 = null;
        CompareInfo = "Select two countries to compare";
        PendingDecision = null;
        HasPendingDecision = false;
        
        UpdateChartData();
        UpdateTradeInfo();
        UpdateWarInfo();
        UpdateEconomyInfo();
    }

    [RelayCommand]
    private void NextTurn()
    {
        if (IsSimulationRunning) return;
        
        _simulation.NextTurn();
        WorldStatus = _simulation.GetWorldStatus();
        OnPropertyChanged(nameof(Countries));
        OnPropertyChanged(nameof(FilteredCountries));
        OnPropertyChanged(nameof(Events));
        OnPropertyChanged(nameof(ActiveWars));
        
        CheckPendingDecision();
        
        if (SelectedCountry != null)
        {
            UpdateSelectedCountryInfo();
        }
        
        UpdateChartData();
        UpdateTradeInfo();
        UpdateWarInfo();
        UpdateEconomyInfo();
    }

    [RelayCommand]
    private void SetSpeed(string speedStr)
    {
        if (int.TryParse(speedStr, out int speed))
        {
            SimulationSpeed = speed;
        }
    }

    [RelayCommand]
    private void SetDifficulty(string difficulty)
    {
        Difficulty = difficulty;
        _simulation.Settings = difficulty switch
        {
            "Easy" => GameSettings.Easy(),
            "Hard" => GameSettings.Hard(),
            _ => GameSettings.Normal()
        };
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        ThemeService.Instance.SetTheme(IsDarkTheme);
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        ThemeService.Instance.SetTheme(value);
    }

    [RelayCommand]
    private void SelectPlayerCountry(Country? country)
    {
        if (country == null) return;
        
        PlayerCountryId = country.Id;
        _simulation.PlayerCountryId = country.Id;
        WorldStatus = $"You are now playing as {country.Name}";
    }

    [RelayCommand]
    private void ConfirmPlayerCountry()
    {
        if (SelectedCountry == null)
        {
            PlayerActionResult = "Please select a country first";
            return;
        }
        
        PlayerCountryId = SelectedCountry.Id;
        PlayerCountryDisplay = SelectedCountry.Name;
        _simulation.PlayerCountryId = SelectedCountry.Id;
        WorldStatus = $"You are now playing as {SelectedCountry.Name}";
        PlayerActionResult = $"You are now playing as {SelectedCountry.Name}";
        SavePlayerCountryData();
    }

    [RelayCommand]
    private void RemovePlayerCountry()
    {
        if (string.IsNullOrEmpty(PlayerCountryId))
        {
            PlayerActionResult = "No player country selected";
            return;
        }
        
        var countryName = _simulation.GetCountry(PlayerCountryId)?.Name ?? "Country";
        PlayerCountryId = null;
        PlayerCountryDisplay = "";
        _simulation.PlayerCountryId = null;
        WorldStatus = "You are no longer controlling any country";
        PlayerActionResult = $"You have removed {countryName} from your control";
    }

    [RelayCommand]
    private void SavePlayerCountryData()
    {
        if (string.IsNullOrEmpty(PlayerCountryId))
        {
            PlayerActionResult = "No player country to save";
            return;
        }
        
        try
        {
            var country = _simulation.GetCountry(PlayerCountryId);
            if (country == null)
            {
                PlayerActionResult = "Player country not found";
                return;
            }
            
            var playerData = new PlayerCountryData
            {
                Country = country,
                CurrentTurn = _simulation.CurrentTurn,
                SavedAt = DateTime.Now
            };
            
            var json = JsonSerializer.Serialize(playerData, WorldSimJsonContext.Default.PlayerCountryData);
            File.WriteAllText(SavePathManager.PlayerDataPath, json);
            PlayerActionResult = $"Player country data saved";
        }
        catch (Exception ex)
        {
            PlayerActionResult = $"Save failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SelectDecisionOption(string optionId)
    {
        if (PendingDecision == null) return;
        
        PendingDecision.SelectedOptionId = optionId;
        PendingDecision.IsResolved = true;
        HasPendingDecision = false;
        PendingDecision = null;
        
        _simulation.ProcessDecisions();
    }

    [RelayCommand]
    private void SaveGame()
    {
        try
        {
            var state = _simulation.SaveGame();
            var json = JsonSerializer.Serialize(state, WorldSimJsonContext.Default.GameState);
            File.WriteAllText(SavePathManager.GameSavePath, json);
            WorldStatus = "Game saved";
        }
        catch (Exception ex)
        {
            WorldStatus = $"Save failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private void LoadGame()
    {
        try
        {
            if (!File.Exists(SavePathManager.GameSavePath))
            {
                WorldStatus = "No save file found";
                return;
            }
            
            var json = File.ReadAllText(SavePathManager.GameSavePath);
            var state = JsonSerializer.Deserialize(json, WorldSimJsonContext.Default.GameState);
            
            if (state != null)
            {
                _simulation.LoadGame(state);
                PlayerCountryId = _simulation.PlayerCountryId;
                PlayerCountryDisplay = _simulation.GetCountry(_simulation.PlayerCountryId ?? "")?.Name ?? "";
                
                OnPropertyChanged(nameof(Countries));
                OnPropertyChanged(nameof(FilteredCountries));
                OnPropertyChanged(nameof(Events));
                OnPropertyChanged(nameof(ActiveWars));
                OnPropertyChanged(nameof(Organizations));
                
                WorldStatus = $"Game loaded from turn {state.Turn}";
                UpdateChartData();
                UpdateTradeInfo();
                UpdateWarInfo();
                UpdateEconomyInfo();
            }
        }
        catch (Exception ex)
        {
            WorldStatus = $"Load failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        OnPropertyChanged(nameof(FilteredCountries));
    }

    [RelayCommand]
    private void CompareCountries()
    {
        UpdateCompareInfo();
    }

    [RelayCommand]
    private void ToggleEventSort()
    {
        EventSortAscending = !EventSortAscending;
        OnPropertyChanged(nameof(Events));
    }

    [RelayCommand]
    private void FormAlliance()
    {
        if (string.IsNullOrEmpty(PlayerCountryId) || ActionTargetCountry == null) 
        {
            PlayerActionResult = "Select a target country first";
            return;
        }
        
        var player = _simulation.GetCountry(PlayerCountryId);
        if (player == null) 
        {
            PlayerActionResult = "You must select a player country first";
            return;
        }
        
        if (!player.Allies.Contains(ActionTargetCountry.Id))
        {
            player.Allies.Add(ActionTargetCountry.Id);
            ActionTargetCountry.Allies.Add(player.Id);
            player.Enemies.Remove(ActionTargetCountry.Id);
            ActionTargetCountry.Enemies.Remove(player.Id);
            PlayerActionResult = $"Alliance formed with {ActionTargetCountry.Name}";
            OnPropertyChanged(nameof(Countries));
            OnPropertyChanged(nameof(FilteredCountries));
            UpdateSelectedCountryInfo();
        }
        else
        {
            PlayerActionResult = $"Already allied with {ActionTargetCountry.Name}";
        }
    }

    [RelayCommand]
    private void BreakAlliance()
    {
        if (string.IsNullOrEmpty(PlayerCountryId) || ActionTargetCountry == null) 
        {
            PlayerActionResult = "Select a target country first";
            return;
        }
        
        var player = _simulation.GetCountry(PlayerCountryId);
        if (player == null) 
        {
            PlayerActionResult = "You must select a player country first";
            return;
        }
        
        if (player.Allies.Contains(ActionTargetCountry.Id))
        {
            player.Allies.Remove(ActionTargetCountry.Id);
            ActionTargetCountry.Allies.Remove(player.Id);
            PlayerActionResult = $"Alliance broken with {ActionTargetCountry.Name}";
            OnPropertyChanged(nameof(Countries));
            OnPropertyChanged(nameof(FilteredCountries));
            UpdateSelectedCountryInfo();
        }
        else
        {
            PlayerActionResult = $"Not allied with {ActionTargetCountry.Name}";
        }
    }

    [RelayCommand]
    private void DeclareWar()
    {
        if (string.IsNullOrEmpty(PlayerCountryId) || ActionTargetCountry == null) 
        {
            PlayerActionResult = "Select a target country first";
            return;
        }
        
        var player = _simulation.GetCountry(PlayerCountryId);
        if (player == null) 
        {
            PlayerActionResult = "You must select a player country first";
            return;
        }
        
        if (!player.Enemies.Contains(ActionTargetCountry.Id))
        {
            player.Enemies.Add(ActionTargetCountry.Id);
            ActionTargetCountry.Enemies.Add(player.Id);
            player.Allies.Remove(ActionTargetCountry.Id);
            ActionTargetCountry.Allies.Remove(player.Id);
            PlayerActionResult = $"War declared on {ActionTargetCountry.Name}";
            
            _simulation.Events.Insert(0, new SimulationEvent
            {
                Turn = _simulation.CurrentTurn,
                CountryId = player.Id,
                CountryName = player.Name,
                Type = "Military",
                Title = "War Declared!",
                Description = $"{player.Name} has declared war on {ActionTargetCountry.Name}!"
            });
            
            OnPropertyChanged(nameof(Countries));
            OnPropertyChanged(nameof(FilteredCountries));
            OnPropertyChanged(nameof(Events));
            UpdateSelectedCountryInfo();
            UpdateWarInfo();
        }
        else
        {
            PlayerActionResult = $"Already at war with {ActionTargetCountry.Name}";
        }
    }

    [RelayCommand]
    private void ApplyEconomicPolicy()
    {
        if (string.IsNullOrEmpty(PlayerCountryId)) 
        {
            PlayerActionResult = "You must select a player country first";
            return;
        }
        
        var player = _simulation.GetCountry(PlayerCountryId);
        if (player == null) 
        {
            PlayerActionResult = "You must select a player country first";
            return;
        }
        
        switch (SelectedPolicy)
        {
            case "Tax Increase":
                player.Gdp *= 1.02;
                player.Happiness -= 3;
                player.Stability -= 2;
                PlayerActionResult = "Taxes increased - GDP +2%, Happiness -3, Stability -2";
                break;
            case "Tax Cut":
                player.Gdp *= 0.98;
                player.Happiness += 3;
                player.Stability += 1;
                player.Gdp *= 1.02;
                PlayerActionResult = "Taxes cut - Happiness +3, Stability +1, short-term GDP -2%";
                break;
            case "Military Buildup":
                player.MilitaryPower = (int)(player.MilitaryPower * 1.15);
                player.Gdp *= 0.98;
                PlayerActionResult = "Military expanded by 15%, GDP -2%";
                break;
            case "Demilitarize":
                player.MilitaryPower = (int)(player.MilitaryPower * 0.85);
                player.Gdp *= 1.02;
                player.Happiness += 2;
                PlayerActionResult = "Military -15%, GDP +2%, Happiness +2";
                break;
            case "Welfare":
                player.Happiness += 5;
                player.Stability += 3;
                player.Gdp *= 0.97;
                PlayerActionResult = "Welfare: Happiness +5, Stability +3, GDP -3%";
                break;
            case "Education":
                player.Gdp *= 1.05;
                player.Stability += 2;
                PlayerActionResult = "Education: Long-term GDP +5%, Stability +2";
                break;
            default:
                PlayerActionResult = "No policy selected";
                break;
        }
        
        player.Happiness = Math.Clamp(player.Happiness, 0, 100);
        player.Stability = Math.Clamp(player.Stability, 0, 100);
        
        OnPropertyChanged(nameof(Countries));
        OnPropertyChanged(nameof(FilteredCountries));
        UpdateSelectedCountryInfo();
        UpdateChartData();
        UpdateEconomyInfo();
    }

    [RelayCommand]
    private void EstablishTradeRoute()
    {
        if (string.IsNullOrEmpty(PlayerCountryId) || ActionTargetCountry == null) 
        {
            PlayerActionResult = "Select a target country first";
            return;
        }
        
        var player = _simulation.GetCountry(PlayerCountryId);
        if (player == null) 
        {
            PlayerActionResult = "You must select a player country first";
            return;
        }
        
        var existingRoute = _simulation.TradeRoutes.FirstOrDefault(t => 
            (t.ExporterId == player.Id && t.ImporterId == ActionTargetCountry.Id) ||
            (t.ExporterId == ActionTargetCountry.Id && t.ImporterId == player.Id));
        
        if (existingRoute == null)
        {
            _simulation.TradeRoutes.Add(new TradeRoute
            {
                ExporterId = player.Id,
                ImporterId = ActionTargetCountry.Id,
                ResourceId = "general",
                Amount = 10,
                Value = 100,
                TurnEstablished = _simulation.CurrentTurn
            });
            PlayerActionResult = $"Trade route established with {ActionTargetCountry.Name}";
            OnPropertyChanged(nameof(Countries));
            UpdateTradeInfo();
        }
        else
        {
            PlayerActionResult = $"Trade route already exists with {ActionTargetCountry.Name}";
        }
    }

    private List<SimulationEvent> GetSortedEvents()
    {
        var events = _simulation.Events.ToList();
        if (EventSortAscending)
        {
            return events.OrderBy(e => e.Turn).ToList();
        }
        return events.OrderByDescending(e => e.Turn).ToList();
    }

    private void CheckPendingDecision()
    {
        var decisions = _simulation.GetPendingDecisions();
        if (decisions.Count > 0)
        {
            PendingDecision = decisions[0];
            HasPendingDecision = true;
        }
    }

    public void SelectCountry(Country? country)
    {
        SelectedCountry = country;
        if (country != null)
        {
            SelectedCountryName = country.Name;
            UpdateSelectedCountryInfo();
        }
        else
        {
            SelectedCountryName = "";
            SelectedCountryInfo = "Select a country to view details";
            CountryDetails.Clear();
        }
    }

    public void SetCompareCountry2(Country? country)
    {
        CompareCountry2 = country;
        UpdateCompareInfo();
    }
}
