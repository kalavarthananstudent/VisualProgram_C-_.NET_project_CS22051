namespace ABI_VISUAL.Models
{
    /// <summary>Settings passed between forms via the Settings dialog.</summary>
    public class VisualizerSettings
    {
        // Sorting
        public int ArraySize       { get; set; } = 60;
        public int SortSpeedMs     { get; set; } = 30;   // timer interval ms

        // Pathfinding
        public int GridRows        { get; set; } = 25;
        public int GridCols        { get; set; } = 45;
        public int PathSpeedMs     { get; set; } = 20;

        public VisualizerSettings Clone() => (VisualizerSettings)MemberwiseClone();
    }
}
