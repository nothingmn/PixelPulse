using System.Reflection.Metadata.Ecma335;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PixelPulse.Pages;
using WLEDAnimated;
using WLEDAnimated.Interfaces;

namespace PixelPulse.Models.ViewModels;

public partial class WledDeviceViewModel : ObservableObject
{
    private WLEDDevice _device;
    private readonly IWLEDApiManager _apiManager;

    public WledDeviceViewModel(WLEDDevice device, IWLEDApiManager apiManager)
    {
        _apiManager = apiManager;
        UpdateState(device);
    }

    private void UpdateState(WLEDDevice device)
    {
        _device = device;
        NetworkAddress = device.NetworkAddress;
        Name = device.Name;
        Width = device.Width;
        Height = device.Height;
        Is2D = device.Is2D;

        Brightness = device.Brightness;
        IsOn = device.WledDevice.State.On;
        NetworkAddressUri = $"http://{NetworkAddress}";
        EndpointName = $"{Name} @ {NetworkAddressUri}";
        ProductName = $"Pixel Pulse - {EndpointName}";
        if (device.LedCount.HasValue) LedCount = device.LedCount.Value;
        if (device.PowerUsage.HasValue) PowerUsage = device.PowerUsage.Value;
        LedDetails = $"({LedCount}, {PowerUsage}mA";
        if (Is2D)
        {
            LedDetails += $", 2D={Width}x{Height})";
        }
        else
        {
            LedDetails += ", 1D)";
        }
        OnOffText = IsOn ? "Turn Off" : "Turn On";
    }

    [RelayCommand]
    private async Task ToggleLight()
    {
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);
        var request = _apiManager.ConvertStateResponseToRequest(_device.WledDevice.State);
        request.On = !request.On;
        await _apiManager.SetStateFromRequest(request);
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);
        UpdateState(_device);
    }

    [RelayCommand]
    private async Task ChangeBrightness()
    {
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);
        var request = _apiManager.ConvertStateResponseToRequest(_device.WledDevice.State);
        request.Brightness = (byte)this.brightness;
        await _apiManager.SetStateFromRequest(request);
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);
        UpdateState(_device);
    }

    [RelayCommand]
    private async Task ViewManage()
    {
        //await App.Current.MainPage.Navigation.PushAsync(new ManageWebUIPage() { SelectedWledDeviceViewModel = this });
        var navigationParameter = new ShellNavigationQueryParameters
        {
            { "SelectedWledDeviceViewModel", this }
        };
        await Shell.Current.GoToAsync($"managepage", navigationParameter);
    }

    //[RelayCommand]
    //private async Task ViewManage()
    //{
    //    //await App.Current.MainPage.Navigation.PushAsync(new ManageWebUIPage() { SelectedWledDeviceViewModel = this });
    //    var navigationParameter = new ShellNavigationQueryParameters
    //    {
    //        { "SelectedWledDeviceViewModel", this }
    //    };
    //    await Shell.Current.GoToAsync($"managedevicepage", navigationParameter);
    //}

    [RelayCommand]
    private async Task GoBackHome()
    {
        //await App.Current.MainPage.Navigation.PopAsync();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private Task View2D()
    {
        return Task.CompletedTask;
    }

    [ObservableProperty] private string networkAddressUri;

    [ObservableProperty] private string networkAddress;

    [ObservableProperty] private string name;

    [ObservableProperty] private string productName;

    [ObservableProperty] private string endpointName;

    [ObservableProperty] private string ledDetails;

    [ObservableProperty] private int? width;

    [ObservableProperty] private int? height;

    [ObservableProperty] private bool is2D;

    [ObservableProperty] private int brightness;

    [ObservableProperty] private bool isOn;

    [ObservableProperty] private string onOffText;

    [ObservableProperty] private int ledCount;

    [ObservableProperty] private int powerUsage;
}