using System.Windows.Input;
using AbleSharp.GUI.Services;
using AbleSharp.GUI.ViewModels;
using AbleSharp.SDK; 

namespace AbleSharp.GUI.Commands
{
    public class OpenProjectCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public OpenProjectCommand(MainWindowViewModel vm)
        {
            _mainWindowViewModel = vm;
        }

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            var filePath = await FileDialogService.ShowOpenFileDialogAsync();

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    var project = AbletonProjectHandler.LoadFromFile(filePath);

                    var projectViewModel = new ProjectViewModel(project);

                    _mainWindowViewModel.ShowProjectView(projectViewModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading project: {ex.Message}");
                }
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}