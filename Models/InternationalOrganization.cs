using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldSimApp.Models;

public class InternationalOrganization
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("founderId")]
    public string? FounderId { get; set; }

    [JsonPropertyName("headquartersId")]
    public string? HeadquartersId { get; set; }

    [JsonPropertyName("memberIds")]
    public List<string> MemberIds { get; set; } = new();

    [JsonPropertyName("requiredStability")]
    public double RequiredStability { get; set; } = 50;

    [JsonPropertyName("effectMilitary")]
    public double EffectMilitary { get; set; } = 0;

    [JsonPropertyName("effectTrade")]
    public double EffectTrade { get; set; } = 0;

    [JsonPropertyName("effectStability")]
    public double EffectStability { get; set; } = 0;

    [JsonPropertyName("securityCouncilIds")]
    public List<string> SecurityCouncilIds { get; set; } = new();

    [JsonPropertyName("actions")]
    public List<string> Actions { get; set; } = new();

    [JsonPropertyName("defensePact")]
    public bool DefensePact { get; set; }
}

public class OrganizationData
{
    [JsonPropertyName("organizations")]
    public List<InternationalOrganization> Organizations { get; set; } = new();
}
