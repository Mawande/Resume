using System.Net.Http.Json;
using System.Text.Json;
using ResumeSite.Models;

namespace ResumeSite.Services;

/// <summary>Loads wwwroot/data/resume.json once and shares it with every component.</summary>
public sealed class ResumeService(HttpClient http)
{
    private Task<Resume>? _resume;

    /// <summary>Returns the resume, or throws with a descriptive message if the file is missing or invalid.</summary>
    public Task<Resume> GetAsync() => _resume ??= LoadAsync();

    /// <summary>Returns the resume, or null on any failure (used by header/footer, which just render less).</summary>
    public async Task<Resume?> TryGetAsync()
    {
        try
        {
            return await GetAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<Resume> LoadAsync()
    {
        var resume = await http.GetFromJsonAsync("data/resume.json", ResumeJsonContext.Default.Resume);
        return resume ?? throw new JsonException("resume.json is empty.");
    }
}