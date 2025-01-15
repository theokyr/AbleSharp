using System.Collections.ObjectModel;
using System.Collections.Generic;
using AbleSharp.Lib;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.ViewModels;

public class ProjectViewModel
{
    private readonly ILogger<ProjectViewModel> _logger;

    public ObservableCollection<TrackViewModel> RootTracks { get; set; } = new();
    public TimelineViewModel TimelineVM { get; }

    public ProjectViewModel(AbletonProject project)
    {
        _logger = LoggerService.GetLogger<ProjectViewModel>();

        _logger.LogInformation("Creating ProjectViewModel for loaded project");

        if (project?.LiveSet?.Tracks != null) BuildTrackHierarchy(project.LiveSet.Tracks);

        TimelineVM = new TimelineViewModel(project);
    }

    private void BuildTrackHierarchy(List<Track> tracks)
    {
        var all = new Dictionary<int, TrackViewModel>();
        foreach (var t in tracks)
        {
            var tvm = new TrackViewModel(t);
            all[t.Id] = tvm;
        }

        foreach (var t in tracks)
        {
            var tvm = all[t.Id];
            var parentId = t.TrackGroupId?.Val ?? -1;
            if (parentId == -1)
            {
                RootTracks.Add(tvm);
            }
            else
            {
                if (all.ContainsKey(parentId))
                    all[parentId].Children.Add(tvm);
                else
                    RootTracks.Add(tvm);
            }
        }

        _logger.LogDebug("Built track hierarchy with {Count} root tracks", RootTracks.Count);
    }
}