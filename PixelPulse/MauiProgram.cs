using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PixelPulse.Models.ViewModels;
using PixelPulse.Pages;
using WLEDAnimated;
using WLEDAnimated.Animation;
using WLEDAnimated.Interfaces;
using WLEDAnimated.Interfaces.Services;

namespace PixelPulse
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })

                ;

            builder.Services.AddKeyedTransient<IImageConverter, ImageToTPM2NETConverter>("TPM2NET");
            builder.Services.AddKeyedTransient<IImageConverter, ImageToDNRGBConverter>("DNRGB");

            builder.Services.AddTransient<IImageResizer, ImageSharpImageResizer>();

            builder.Services.AddTransient<IImageToConverterFactory, ImageToConverterFactory>();
            builder.Services.AddTransient<IImageSender, ImageUDPSender>();
            builder.Services.AddTransient<WLEDDevice, WLEDDevice>();

            builder.Services.AddTransient<IWLEDApiManager, WLEDApiManager>();

            builder.Services.AddTransient<MultiStep, MultiStep>();
            builder.Services.AddTransient<DisplayImageStep, DisplayImageStep>();
            builder.Services.AddTransient<WLEDStateStep, WLEDStateStep>();
            builder.Services.AddTransient<Manage2DDeviceViewModel, Manage2DDeviceViewModel>();
            builder.Services.AddTransient<WledDevicesViewModel, WledDevicesViewModel>();
            builder.Services.AddTransient<WledDeviceViewModel, WledDeviceViewModel>();

            builder.Services.AddTransient<DisplayTextStep, DisplayTextStep>();
            builder.Services.AddTransient<DisplayRenderedWeatherImageStep, DisplayRenderedWeatherImageStep>();

            builder.Services.AddSingleton<DeviceDiscovery>();
            builder.Services.AddSingleton<WledDeviceDiscovery>();
            builder.Services.AddSingleton<WLEDDeviceManager>();
            builder.Services.AddTransient<EndpointConverter, EndpointConverter>();

            builder.Services.AddTransient<WLEDAnimationLoader>();
            builder.Services.AddTransient<AnimationManager>();
            builder.Services.AddTransient<AssemblyTypeProcessor>();

            builder.Services.AddTransient<IScrollingTextPluginFactory, ScrollingTextPluginFactory>();
            LoadScrollingTextPlugins(builder.Services);
            RegisterServices(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            RegisterRoutes();

            App = builder.Build();

            var dd = App.Services.GetService<WledDeviceDiscovery>();
            var discover = App.Services.GetService<DeviceDiscovery>();
            dd.Start(discover);

            return App;
        }

        private static void RegisterRoutes()
        {
            Routing.RegisterRoute("managepage", typeof(ManageWebUIPage));
            Routing.RegisterRoute("manage2dpage", typeof(Manage2DPage));
        }

        public static MauiApp App { get; set; }

        private static void RegisterServices(IServiceCollection services)
        {
            //services.AddTransient<IWeather, Weather>();
            //services.AddTransient<IWeatherResponse, WeatherResponse>();
            //services.AddTransient<ISeries, Series>();
            //services.AddTransient<IWind10m, Wind10m>();
        }

        private static void LoadScrollingTextPlugins(IServiceCollection services)
        {
            var asmFile = new FileInfo(typeof(MauiProgram).Assembly.Location);
            var binFolder = new DirectoryInfo(System.IO.Path.Combine(asmFile.Directory.FullName));

            var loader = new AssemblyTypeProcessor();
            var pluginTypes = loader.ProcessTypesImplementingInterface(binFolder.FullName, typeof(IScrollingTextPlugin));

            foreach (var pluginType in pluginTypes)
            {
                services.AddKeyedTransient(typeof(IScrollingTextPlugin), pluginType.Name, pluginType);
                services.AddTransient(typeof(IScrollingTextPlugin), pluginType);
                Console.WriteLine($"Added plugin {pluginType.Name}");
            }
        }
    }
}