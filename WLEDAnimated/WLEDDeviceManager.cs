namespace WLEDAnimated;

public delegate void WLEDDeviceAddedEventHandler(WLEDDeviceManager sender, WLEDDevice e);

public class WLEDDeviceManager
{
    public event WLEDDeviceAddedEventHandler? WLEDDeviceAdded;

    public List<WLEDDevice> WLEDDevices { get; set; } = new List<WLEDDevice>();

    private SpinLock _spinLock = new SpinLock();

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