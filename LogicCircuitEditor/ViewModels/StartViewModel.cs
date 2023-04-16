using LogicCircuitEditor.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;

namespace LogicCircuitEditor.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private int index;
        private ObservableCollection<ProjectFile> projects;

        public StartViewModel()
        {
            Projects = new ObservableCollection<ProjectFile>();
            Index = -1;
        }


        public ObservableCollection<ProjectFile> Projects
        {
            get => projects;
            set => this.RaiseAndSetIfChanged(ref projects, value);
        }
        public int Index
        {
            get => index;
            set => this.RaiseAndSetIfChanged(ref index, value);
        }
    }
}