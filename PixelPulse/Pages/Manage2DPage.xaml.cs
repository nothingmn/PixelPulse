using PixelPulse.Models.ViewModels;

namespace PixelPulse.Pages;

[QueryProperty(nameof(Manage2DDeviceViewModel), "Manage2DDeviceViewModel")]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Manage2DPage : ContentPage
{
    private Manage2DDeviceViewModel selectedManage2DDeviceViewModel;

    public Manage2DDeviceViewModel Manage2DDeviceViewModel
    {
        get => selectedManage2DDeviceViewModel;
        set
        {
            selectedManage2DDeviceViewModel = value;
            BindingContext = value;
            OnPropertyChanged();
        }
    }

    public Manage2DPage()
    {
        InitializeComponent();
    }
}