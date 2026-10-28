namespace ResumeSite.Services;

public sealed record PageSection(string Id, string Label);

/// <summary>The page's sections, in order. The header links and the scroll tracking both use this list.</summary>
public static class PageSections
{
    public static readonly IReadOnlyList<PageSection> All =
    [
        new("introduction", "Introduction"),
        new("experience", "Skills & Experience"),
        new("about", "About"),
    ];
}