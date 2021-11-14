namespace Visualization;

internal sealed class VisualizationConfig
{
    private static readonly VisualizationConfig _instance = new();

    public static VisualizationConfig Instance => _instance;


    private VisualizationConfig()
    {
    }

    public bool DrawRanges { get; set; }
}
