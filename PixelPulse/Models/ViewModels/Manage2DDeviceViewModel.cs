using System.Net;
using System.Reflection.Metadata.Ecma335;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PixelPulse.Pages;
using WLEDAnimated;
using WLEDAnimated.Interfaces;
using Size = SixLabors.ImageSharp.Size;

namespace PixelPulse.Models.ViewModels;

public partial class Manage2DDeviceViewModel : ObservableObject

{
    private WLEDDevice _device;
    private readonly IWLEDApiManager _apiManager;
    private readonly IScrollingTextPluginFactory _scrollingTextPluginFactory;
    private readonly IImageSender _sender;

    public Manage2DDeviceViewModel(WLEDDevice device, IWLEDApiManager apiManager)
    {
        _apiManager = apiManager;
        _scrollingTextPluginFactory = MauiProgram.App.Services.GetService<IScrollingTextPluginFactory>();
        this.ScrollingTextPlugins = _scrollingTextPluginFactory.GetAllPlugins();
        _sender = MauiProgram.App.Services.GetService<IImageSender>();
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
        ProductName = EndpointName;
        if (device.LedCount.HasValue) LedCount = device.LedCount.Value;
        if (device.PowerUsage.HasValue) PowerUsage = device.PowerUsage.Value;
        if (Is2D)
        {
            LedDetails = $"({Width}x{Height}={LedCount}, {PowerUsage}mA)";
        }
        else
        {
            LedDetails = $"({LedCount}, {PowerUsage}mA)";
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
        request.Brightness = (byte)Brightness;
        await _apiManager.SetStateFromRequest(request);
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);
        UpdateState(_device);
    }

    [RelayCommand]
    private async Task SendScrollingText()
    {
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);

        if (SelectedTextPlugin == null)
        {
            await _apiManager.ScrollingText(this.ScrollingText, Speed, YOffset, Trail, FontSize, Rotate);
        }
        else
        {
            await _apiManager.ScrollingText(SelectedTextPlugin.GetType().Name, this.scrollingText, Speed, YOffset, Trail, FontSize, Rotate);
        }

        UpdateState(_device);
    }

    [RelayCommand]
    private async Task SendLocalImage()
    {
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);

        //Bmp
        //    Gif
        //Jpeg
        //    Pbm
        //Png
        //    Tiff
        //Tga
        //    WebP
        var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "public.image" } },
            { DevicePlatform.Android, new[]
            {
                "image/jpg",
                "image/png",
                "image/gif",
                "image/bmp",
                "image/jpeg",
                "image/pbm",
                "image/tiff",
                "image/tga",
                "image/webp"
            } },
            { DevicePlatform.UWP, new[] { ".jpg", ".png", ".gif", ".bmp", ".jpeg", ".pbm", ".tiff", ".tga", ".webp" } },
        });
        PickOptions options = new()
        {
            PickerTitle = "Please select your artwork",
            FileTypes = fileTypes,
        };
        var result = await PickAndShow(options);

        if (result != null)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    _sender.Send(_device.NetworkAddress, 21324, result.FullPath, new Size(_device.Width.Value, _device.Height.Value), StartIndex, (byte)Wait, PauseBetweenFrames, Iterations);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }

    [RelayCommand]
    private async Task SendUrl()
    {
        _device.WledDevice = await _apiManager.Connect(_device.NetworkAddress);

        var url = this.ImageUrl;

        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
        System.Uri uri;
        if (System.Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            //download image from url
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var filename = Path.GetFileName(uri.LocalPath);
                if (string.IsNullOrWhiteSpace(filename))
                {
                    filename = Guid.NewGuid().ToString();
                    response.Content.Headers.TryGetValues("Content-Type", out var contentTypes);
                    if (contentTypes != null && contentTypes.Any())
                    {
                        var contentType = contentTypes.First();
                        var extension = contentType.Split('/').Last();
                        filename = $"{filename}.{extension}";
                    }
                }

                var filePath = System.IO.Path.Combine(Path.GetTempPath(), filename);
                var fileInfo = new System.IO.FileInfo(filePath);
                if (string.IsNullOrWhiteSpace(fileInfo.Extension)) filePath = $"{filePath}.gif";

                using (var fileStream = System.IO.File.Create(filePath))
                {
                    await stream.CopyToAsync(fileStream);
                }

                try
                {
                    Task.Factory.StartNew(() =>
                    {
                        _sender.Send(_device.NetworkAddress, 21324, filePath,
                            new Size(_device.Width.Value, _device.Height.Value), StartIndex, (byte)Wait, PauseBetweenFrames,
                            Iterations);
                    });
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        UpdateState(_device);
    }

    [ObservableProperty] private string scrollingText = "Hello World!";
    [ObservableProperty] private int speed = 128;
    [ObservableProperty] private int yOffset = 0;
    [ObservableProperty] private int trail = 0;
    [ObservableProperty] private int fontSize = 255;
    [ObservableProperty] private int rotate = 15;
    [ObservableProperty] private IList<IScrollingTextPlugin> scrollingTextPlugins;
    [ObservableProperty] private IScrollingTextPlugin selectedTextPlugin;
    [ObservableProperty] private string imageUrl = "https://media1.tenor.com/m/qjM445k2uREAAAAC/eyeball-creepy.gif";

    [ObservableProperty] private int startIndex = 0;
    [ObservableProperty] private int wait = 5;
    [ObservableProperty] private int pauseBetweenFrames = 100;
    [ObservableProperty] private int iterations = 1;

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