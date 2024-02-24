using System.Collections.ObjectModel;
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
            App.Current.Dispatcher.Dispatch(() =>
                {
                    foreach (var device in _deviceManager.WLEDDevices)
                    {
                        var apiManager = MauiProgram.App.Services.GetService<IWLEDApiManager>();
                        wledDevices.Add(new WledDeviceViewModel(device, apiManager));
                    }
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void _deviceManager_WLEDDeviceAdded(WLEDDeviceManager sender, WLEDDevice e)
    {
        App.Current.Dispatcher.Dispatch(() =>
        {
            var apiManager = MauiProgram.App.Services.GetService<IWLEDApiManager>();
            var vm = new WledDeviceViewModel(e, apiManager);
            WledDevices.Add(vm);
        });
        //var vm = new WledDeviceViewModel(e);
        //WledDevices.Add(vm);
        //OnPropertyChanged("WledDevices");
        //WledDevices = WledDevices;
    }

    [ObservableProperty] private ObservableCollection<WledDeviceViewModel> wledDevices = new();
}