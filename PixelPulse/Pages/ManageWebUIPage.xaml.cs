using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelPulse.Models.ViewModels;

namespace PixelPulse.Pages;

[QueryProperty(nameof(SelectedWledDeviceViewModel), "SelectedWledDeviceViewModel")]
public partial class ManageWebUIPage : ContentPage
{
    private WledDeviceViewModel selectedWledDeviceViewModel;

    public WledDeviceViewModel SelectedWledDeviceViewModel
    {
        get => selectedWledDeviceViewModel;
        set
        {
            selectedWledDeviceViewModel = value;
            BindingContext = SelectedWledDeviceViewModel;
            OnPropertyChanged();
        }
    }

    public ManageWebUIPage()
    {
        InitializeComponent();
    }
}