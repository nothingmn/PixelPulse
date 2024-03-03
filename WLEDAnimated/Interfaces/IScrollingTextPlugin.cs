namespace WLEDAnimated.Interfaces;

public interface IScrollingTextPlugin
{
    string Name { get; }
    string TypeName { get; }

    Task<string> GetTextToDisplay(string payload = null);
}