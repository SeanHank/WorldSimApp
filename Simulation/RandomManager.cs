using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace WorldSimApp.Simulation;

public static class RandomManager
{
    private static Random? _instance;
    private static readonly object _lock = new();
    
    public static Random Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var bytes = new byte[4];
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(bytes);
                        }
                        int seed = BitConverter.ToInt32(bytes, 0);
                        _instance = new Random(seed);
                    }
                }
            }
            return _instance;
        }
    }
    
    public static double NextGaussian(double mean, double stdDev)
    {
        double u1 = 1.0 - Instance.NextDouble();
        double u2 = 1.0 - Instance.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;
    }
    
    public static double NextRange(double min, double max)
    {
        return min + (max - min) * Instance.NextDouble();
    }
    
    public static T WeightedRandom<T>(Dictionary<T, double> weights) where T : notnull
    {
        if (weights == null || weights.Count == 0)
            throw new ArgumentException("Weights dictionary cannot be empty");
            
        double total = weights.Values.Sum();
        double r = Instance.NextDouble() * total;
        double cumulative = 0;
        foreach (var kvp in weights)
        {
            cumulative += kvp.Value;
            if (r <= cumulative) return kvp.Key;
        }
        return weights.Keys.First();
    }
    
    public static bool Chance(double probability)
    {
        return Instance.NextDouble() < probability;
    }
    
    public static int NextInt(int min, int max)
    {
        return Instance.Next(min, max);
    }
}
