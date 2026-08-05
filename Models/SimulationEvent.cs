namespace WorldSimApp.Models;

public class SimulationEvent
{
    public int Turn { get; set; }
    public string CountryId { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double ImpactGdp { get; set; }
    public double ImpactStability { get; set; }
    public double ImpactMilitary { get; set; }
    public double ImpactHappiness { get; set; }
    public double ImpactDiplomatic { get; set; }

    public string Summary => $"[{Turn}] {CountryName}: {Title}";
}
