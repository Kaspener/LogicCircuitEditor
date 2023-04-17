using LogicCircuitEditor.Models;
using ReactiveUI;
using System.IO;
using YamlDotNet.Serialization;

namespace LogicCircuitEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object content;
        private MainUserControlViewModel mainModel;
        private StartViewModel startModel;
        public MainWindowViewModel()
        {
            mainModel = new MainUserControlViewModel();
            startModel = new StartViewModel();
            Content = startModel;
        }
        public object Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }
        public void CreateNewProject()
        {
            mainModel.Project = new Project();
            Content = mainModel;
        }
        public void AddNewProjectPath(string name, string path)
        {
            ProjectFile file = new ProjectFile { Name = name, Path = path };
            ProjectFile find = null;
            foreach (ProjectFile item in startModel.Projects)
            {
                if (item.Path == path) { find = item; break; }
            }
            if (find != null)
            {
                startModel.Projects.Remove(find);
                startModel.Projects.Insert(0, file);
            }
            else
            {
                startModel.Projects.Insert(0, file);
            }
            var serializer = new SerializerBuilder()
                .Build();
            var yaml = serializer.Serialize(startModel.Projects);
            using (StreamWriter writer = new StreamWriter("../../../Assets/projects.yaml", false))
            {
                writer.WriteLine(yaml);
            }
        }
        public void OpenProject(string path)
        {
            if (File.Exists(path))
            {
                Content = mainModel;
                mainModel.Index = 0;
                mainModel.ChangeElements = true;
                mainModel.Project = LogicCircuitEditor.Models.Serializer.Load(path);
                mainModel.ChangeElements = false;
                mainModel.Index = 0;
            }
            else
            {
                startModel.Projects.RemoveAt(startModel.Index);
            }
        }
    }
}