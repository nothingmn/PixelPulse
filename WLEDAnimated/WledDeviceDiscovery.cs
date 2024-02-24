using Microsoft.Extensions.Logging;

namespace WLEDAnimated;

public class WledDeviceDiscovery
{
    private readonly WLEDDeviceManager _deviceManager;
    private readonly ILogger<WledDeviceDiscovery> _log;

    public WledDeviceDiscovery(WLEDDeviceManager deviceManager, ILogger<WledDeviceDiscovery> log)
    {
        _deviceManager = deviceManager;
        _log = log;
    }

    public void Start(DeviceDiscovery deviceDiscovery)
    {
        Task.Factory.StartNew(async () =>
        {
            deviceDiscovery.ValidDeviceFound += (sender, e) =>
            {
                var exists = _deviceManager.GetDeviceByNetworkAddress(e.CreatedDevice.NetworkAddress);
                try
                {
                    //best effort
                    //need to lock the collection first
                    if (exists == null)
                    {
                        _deviceManager.AddDevice(e.CreatedDevice);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"Failed to add discovered device:{e.CreatedDevice.NetworkAddress} {e.CreatedDevice.Name}");
                }
            };
            deviceDiscovery.StartDiscovery();
        }, TaskCreationOptions.LongRunning);
    }
}