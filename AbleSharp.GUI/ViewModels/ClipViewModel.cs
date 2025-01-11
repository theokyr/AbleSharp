using AbleSharp.Lib;

namespace AbleSharp.GUI.ViewModels
{
    public class ClipViewModel
    {
        public Clip Clip { get; }
        public decimal Start => Clip?.CurrentStart?.Val ?? 0;
        public decimal End => Clip?.CurrentEnd?.Val ?? 0;
        public decimal Length => End - Start;
        public string ClipName => Clip?.Name?.Val ?? "(Unnamed Clip)";

        public ClipViewModel(Clip clip)
        {
            Clip = clip;
        }

        public override string ToString()
        {
            return $"Start: {Start}, End: {End}, Length: {Length}, ClipName: {ClipName}";
        }
    }
}