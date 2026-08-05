using System.Collections.Generic;

namespace WorldSimApp.Models;

public class DiplomaticAction
{
    public int Turn { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class PendingPolicy
{
    public double EffectGdp { get; set; }
    public int ImplementationDelay { get; set; }
}
