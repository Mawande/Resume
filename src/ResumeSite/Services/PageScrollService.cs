using Microsoft.JSInterop;

namespace ResumeSite.Services;

/// <summary>
/// Bridges Blazor and wwwroot/js/site.js: smooth scrolling, active-section tracking,
/// the header's "scrolled" state and the fade-in animations.
/// </summary>
public sealed class PageScrollService(IJSRuntime js) : IAsyncDisposable
{
    private IJSObjectReference? _module;
    private DotNetObjectReference<PageScrollService>? _selfRef;

    public string? ActiveSectionId { get; private set; }

    public event Action? ActiveSectionChanged;

    /// <summary>Call once the sections are rendered.</summary>
    public async Task InitializeAsync()
    {
        var module = await GetModuleAsync();
        _selfRef ??= DotNetObjectReference.Create(this);
        var ids = PageSections.All.Select(s => s.Id).ToArray();
        await module.InvokeVoidAsync("initialize", _selfRef, ids);
    }

    public async Task ScrollToAsync(string elementId, bool updateHash = true)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("scrollToSection", elementId, updateHash);
    }

    [JSInvokable]
    public void OnActiveSectionChanged(string sectionId)
    {
        if (sectionId == ActiveSectionId) return;
        ActiveSectionId = sectionId;
        ActiveSectionChanged?.Invoke();
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync() =>
        _module ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/site.js");

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try
            {
                await _module.InvokeVoidAsync("dispose");
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Page is closing; nothing to clean up.
            }
        }

        _selfRef?.Dispose();
    }
}