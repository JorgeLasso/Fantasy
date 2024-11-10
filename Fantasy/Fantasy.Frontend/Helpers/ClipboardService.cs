using Microsoft.JSInterop;

namespace Fantasy.Frontend.Helpers;

public class ClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardService(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
    }

    public async Task CopyToClipboardAsync(string text)
    {
        await _jsRuntime.InvokeVoidAsync("copyToClipboard", text);
    }
}