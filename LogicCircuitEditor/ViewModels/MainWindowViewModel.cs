using LogicCircuitEditor.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

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

    }
}