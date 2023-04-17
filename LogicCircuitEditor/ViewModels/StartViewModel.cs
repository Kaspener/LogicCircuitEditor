using LogicCircuitEditor.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using YamlDotNet.Serialization;

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
            Serialize();
        }

        private void Serialize()
        {
            var deserializer = new DeserializerBuilder()
                .Build();
            string input;
            try
            {
                using (StreamReader reader = new StreamReader("../../../Assets/projects.yaml"))
                {
                    input = reader.ReadToEnd();
                }
                Projects = deserializer.Deserialize<ObservableCollection<ProjectFile>>(input);
            }
            catch { }
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