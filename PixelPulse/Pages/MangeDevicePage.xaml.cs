using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelPulse.Models.ViewModels;

namespace PixelPulse.Pages;

[QueryProperty(nameof(SelectedWledDeviceViewModel), "SelectedWledDeviceViewModel")]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MangeDevicePage : TabbedPage
{
    private WledDeviceViewModel selectedWledDeviceViewModel;

    public WledDeviceViewModel SelectedWledDeviceViewModel
    {
        get => selectedWledDeviceViewModel;
        set
        {
            selectedWledDeviceViewModel = value;
            BindingContext = value;
            OnPropertyChanged();
        }
    }

    public MangeDevicePage()
    {
        //InitializeComponent();
    }
}