namespace WLEDAnimated;

public delegate void WLEDDeviceAddedEventHandler(WLEDDeviceManager sender, WLEDDevice e);

public class WLEDDeviceManager
{
    private readonly IServiceProvider _services;

    public event WLEDDeviceAddedEventHandler? WLEDDeviceAdded;

    public List<WLEDDevice> WLEDDevices { get; set; } = new List<WLEDDevice>();

    private SpinLock _spinLock = new SpinLock();

    public WLEDDeviceManager(IServiceProvider services)
    {
        _services = services;
    }

    public async Task AddDeviceByHostOrIp(string hostOrIp)
    {
        var wledDevice = _services.GetService(typeof(WLEDDevice)) as WLEDDevice;
        wledDevice.NetworkAddress = hostOrIp;
        var isActive = await wledDevice.Refresh(); //check if the service is a valid WLED light
        if (isActive)
        {
            wledDevice.Name = wledDevice.WledDevice.Information.Name;
        }

        AddDevice(wledDevice);
    }

    public void AddDevice(WLEDDevice device)
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            WLEDDevices.Add(device);
            if (WLEDDeviceAdded != null)
            {
                WLEDDeviceAdded(this, device);
            }
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }

    public WLEDDevice GetDeviceByNetworkAddress(string networkAddress)
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            return WLEDDevices.FirstOrDefault(d => d.NetworkAddress.Equals(networkAddress, StringComparison.InvariantCultureIgnoreCase));
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }

        return null;
    }

    public WLEDDevice GetDeviceByName(string name)
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            return WLEDDevices.FirstOrDefault(d => d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }

        return null;
    }
}