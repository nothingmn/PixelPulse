using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelPulse.Models.ViewModels;
using WLEDAnimated;

namespace PixelPulse.Pages;

public partial class DevicesListPage : ContentPage
{
    public DevicesListPage()
    {
        InitializeComponent();

        BindingContext = new WledDevicesViewModel();
    }

    private void EntryText_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        (this.BindingContext as WledDevicesViewModel).FilterCommand.ExecuteAsync(e);
    }
}