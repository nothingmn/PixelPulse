using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WLEDAnimated;
using WLEDAnimated.Interfaces;

namespace PixelPulse.Models.ViewModels;

public partial class WledDevicesViewModel : ObservableObject
{
    private WLEDDeviceManager _deviceManager;

    public WledDevicesViewModel()
    {
        _deviceManager = MauiProgram.App.Services.GetService<WLEDDeviceManager>();
        _deviceManager.WLEDDeviceAdded += _deviceManager_WLEDDeviceAdded;

        try
        {
            var viewModels = (from d in _deviceManager.WLEDDevices
                              group d by d.NetworkAddress into deviceGroup
                              select new WledDeviceViewModel(deviceGroup.First(), MauiProgram.App.Services.GetService<IWLEDApiManager>())).ToList();

            WledDevices = viewModels.ToObservableCollection<WledDeviceViewModel>();
            AllWledDevices = viewModels.ToObservableCollection<WledDeviceViewModel>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    [ObservableProperty] private string filterText = "";

    [RelayCommand]
    private async Task Filter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            WledDevices = AllWledDevices;
        }
        else
        {
            WledDevices = (from w in AllWledDevices where w.Name.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase) select w).ToObservableCollection<WledDeviceViewModel>();
        }
    }

    private void _deviceManager_WLEDDeviceAdded(WLEDDeviceManager sender, WLEDDevice e)
    {
        try
        {
            var apiManager = MauiProgram.App.Services.GetService<IWLEDApiManager>();
            var exists = WledDevices.Any(d => d.NetworkAddress.Equals(e.NetworkAddress, StringComparison.InvariantCultureIgnoreCase));
            if (!exists)
            {
                WledDevices.Add(new WledDeviceViewModel(e, apiManager));
            }
            var exists1 = AllWledDevices.Any(d => d.NetworkAddress.Equals(e.NetworkAddress, StringComparison.InvariantCultureIgnoreCase));
            if (!exists)
            {
                AllWledDevices.Add(new WledDeviceViewModel(e, apiManager));
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private ObservableCollection<WledDeviceViewModel> AllWledDevices = new();

    [ObservableProperty] private ObservableCollection<WledDeviceViewModel> wledDevices = new();
}