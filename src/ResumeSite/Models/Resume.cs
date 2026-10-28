using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResumeSite.Models;

/// <summary>Root of wwwroot/data/resume.json.</summary>
public sealed record Resume
{
    public required Profile Profile { get; init; }
    public IReadOnlyList<ContactLink> Contacts { get; init; } = [];
    public IReadOnlyList<EducationEntry> Education { get; init; } = [];
    public IReadOnlyList<SkillGroup> SkillGroups { get; init; } = [];
    public IReadOnlyList<ExperienceEntry> Experience { get; init; } = [];
    public AboutInfo? About { get; init; }
    public FooterInfo? Footer { get; init; }
}

public sealed record Profile
{
    public required string Name { get; init; }
    public required string Title { get; init; }
    public string? Summary { get; init; }
    public string? Location { get; init; }
    public string? Photo { get; init; }
    public string? PhotoAlt { get; init; }
    public string? CvUrl { get; init; }
}

public sealed record ContactLink
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public string? Icon { get; init; }
    public string? Display { get; init; }

    /// <summary>True for web links (opened in a new tab); false for mailto: etc.</summary>
    public bool IsExternal => Url.StartsWith("http", StringComparison.OrdinalIgnoreCase);
}

public sealed record EducationEntry
{
    public required string Qualification { get; init; }
    public required string Institution { get; init; }
    public string? Location { get; init; }
    public string? Start { get; init; }
    public string? End { get; init; }
    public IReadOnlyList<string> Highlights { get; init; } = [];
}

public sealed record SkillGroup
{
    public required string Category { get; init; }
    public IReadOnlyList<Skill> Skills { get; init; } = [];
}

public sealed record Skill
{
    public required string Name { get; init; }
    public string? Icon { get; init; }
}

public sealed record ExperienceEntry
{
    public required string Title { get; init; }
    public required string Company { get; init; }
    public string? CompanyUrl { get; init; }
    public string? Location { get; init; }
    public required string Start { get; init; }
    public string? End { get; init; }
    public string? Description { get; init; }
    public IReadOnlyList<string> Achievements { get; init; } = [];
    public IReadOnlyList<string> Technologies { get; init; } = [];
}

public sealed record AboutInfo
{
    public IReadOnlyList<string> Paragraphs { get; init; } = [];
    public IReadOnlyList<string> Interests { get; init; } = [];
    public CallToAction? CallToAction { get; init; }
}

public sealed record CallToAction
{
    public required string Heading { get; init; }
    public string? Text { get; init; }
    public required string PrimaryUrl { get; init; }
    public required string PrimaryLabel { get; init; }
}

public sealed record FooterInfo
{
    public string? CopyrightName { get; init; }
    public bool ShowBuiltWith { get; init; }
}

/// <summary>
/// Compile-time JSON serializer for resume.json: faster startup and safe with trimming.
/// Strict on purpose, so typos in resume.json produce a clear error instead of silently missing content.
/// </summary>
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    RespectNullableAnnotations = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
[JsonSerializable(typeof(Resume))]
internal partial class ResumeJsonContext : JsonSerializerContext
{
}