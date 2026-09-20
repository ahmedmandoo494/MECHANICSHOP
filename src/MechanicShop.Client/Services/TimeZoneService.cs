using Microsoft.JSInterop;

namespace MechanicShop.Client.Services;

public sealed class TimeZoneService(IJSRuntime js)
{
    private readonly IJSRuntime _js = js;

    public string GetLocalTimeZoneAsync()
    {
         return "Africa/Cairo";
    }
}