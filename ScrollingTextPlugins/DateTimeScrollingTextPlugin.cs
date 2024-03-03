using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;
using Microsoft.Extensions.Logging;

namespace ScrollingTextPlugins;

public class DateTimeScrollingTextPlugin : IScrollingTextPlugin
{
    private readonly ILogger<DateTimeScrollingTextPlugin> _logger;

    public DateTimeScrollingTextPlugin(ILogger<DateTimeScrollingTextPlugin> logger)
    {
        _logger = logger;
    }

    public string Name => "Customizable Date and Time";
    public string TypeName => this.GetType().FullName;

    public async Task<string> GetTextToDisplay(string payload = null)
    {
        var format = payload;
        if (string.IsNullOrWhiteSpace(payload)) format = "dd/MM/yyyy HH:mm:ss";

        _logger.LogInformation("Getting datetime: {format}...", format);

        var final = DateTime.Now.ToString(format);
        _logger.LogInformation(final);
        return final;
    }
}