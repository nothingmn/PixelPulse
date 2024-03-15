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
    [ObservableProperty] private string addNewHost = "";

    [RelayCommand]
    private async Task AddDevice()
    {
        await _deviceManager.AddDeviceByHostOrIp(AddNewHost);
    }

    [RelayCommand]
    private async Task Filter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            WledDevices = AllWledDevices;
        }
        else
        {
            WledDevices = (from w in AllWledDevices
                           where
                w.Name.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase)
                ||
                w.NetworkAddress.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase)
                ||
                w.LedDetails.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase)
                           select w).ToObservableCollection<WledDeviceViewModel>();
        }
    }

    private void _deviceManager_WLEDDeviceAdded(WLEDDeviceManager sender, WLEDDevice e)
    {
        try
        {
            var apiManager = MauiProgram.App.Services.GetService<IWLEDApiManager>();
            WledDevices.Add(new WledDeviceViewModel(e, apiManager));
            AllWledDevices.Add(new WledDeviceViewModel(e, apiManager));
            Deduplication(WledDevices);
            Deduplication(AllWledDevices);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private ObservableCollection<WledDeviceViewModel> Deduplication(ObservableCollection<WledDeviceViewModel> collection)
    {
        // Assuming AllWledDevices is your ObservableCollection<WledDeviceViewModel>
        var uniqueItems = collection
            .GroupBy(device => device.NetworkAddress) // Group by NetworkAddress
            .Select(group => group.First()) // Select the first item from each group
            .ToList(); // Convert to List for easier handling

        collection.Clear(); // Clear the original collection
        foreach (var item in uniqueItems)
        {
            collection.Add(item); // Add back the unique items
        }
        return collection;
    }

    private ObservableCollection<WledDeviceViewModel> AllWledDevices = new();

    [ObservableProperty] private ObservableCollection<WledDeviceViewModel> wledDevices = new();
}