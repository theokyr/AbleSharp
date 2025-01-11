using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AbleSharp.GUI.Commands;
using AbleSharp.GUI.Views;

namespace AbleSharp.GUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object? _currentView;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand OpenProjectCommand { get; }

        public MainWindowViewModel()
        {
            OpenProjectCommand = new OpenProjectCommand(this);
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged();
                }
            }
        }

        // Notifies the UI that a property changed
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void ShowProjectView(ProjectViewModel projectViewModel)
        {
            var projectView = new ProjectView
            {
                DataContext = projectViewModel
            };

            CurrentView = projectView;
        }
    }
}