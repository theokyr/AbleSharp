using System.Reactive;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace AbleSharp.GUI.Commands;

public static class AbleSharpUiCommand
{
    public static ReactiveCommand<Unit, Unit> Create(Action action)
    {
        return ReactiveCommand.Create(
            () => Dispatcher.UIThread.Post(action),
            outputScheduler: AvaloniaScheduler.Instance
        );
    }
}