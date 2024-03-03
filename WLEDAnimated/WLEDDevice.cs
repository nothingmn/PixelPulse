using Kevsoft.WLED;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class WLEDDevice
{
    private readonly IWLEDApiManager _apiManager;

    public int? Width
    {
        get { return _apiManager.Width; }
    }

    public int? Height
    {
        get { return _apiManager.Height; }
    }

    public int? LedCount
    {
        get { return _apiManager.LedCount; }
    }

    public bool Is2D
    {
        get { return _apiManager.Is2D; }
    }

    public int? PowerUsage
    {
        get { return _apiManager.PowerUsage; }
    }

    public int Brightness
    {
        get { return WledDevice.State.Brightness; }
    }

    public WLEDDevice(IWLEDApiManager apiManager)
    {
        _apiManager = apiManager;
    }

    public string NetworkAddress { get; set; }
    public string Name { get; set; }
    public WLedRootResponse WledDevice { get; set; }

    public async Task<bool> Refresh()
    {
        try
        {
            WledDevice = await _apiManager.Connect(NetworkAddress);
            if (WledDevice != null && WledDevice.Information != null) return true;
        }
        catch (Exception ex)
        {
            var x = ex;
        }
        return false;
    }
}