using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public interface IScrollingTextPluginFactory
{
    IScrollingTextPlugin LoadPluginByName(string name);

    IList<IScrollingTextPlugin> GetAllPlugins();
}

public class ScrollingTextPluginFactory : IScrollingTextPluginFactory
{
    private readonly IServiceProvider _provider;

    public ScrollingTextPluginFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IList<IScrollingTextPlugin> GetAllPlugins()
    {
        return _provider.GetServices<IScrollingTextPlugin>().ToList();
    }

    public IScrollingTextPlugin LoadPluginByName(string name)
    {
        IScrollingTextPlugin plugin = null;
        plugin = _provider.GetKeyedService<IScrollingTextPlugin>(name);
        if (plugin == null)
        {
            //maybe they didnt do the full plugin name
            plugin = _provider.GetKeyedService<IScrollingTextPlugin>($"{name}ScrollingTextPlugin");
        }

        return plugin;
    }
}