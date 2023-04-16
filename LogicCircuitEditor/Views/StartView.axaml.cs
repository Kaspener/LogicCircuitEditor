using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace LogicCircuitEditor.Views
{
    public partial class StartView : UserControl
    {
        public StartView()
        {
            InitializeComponent();
        }
        private void CloseWindowButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.GetLogicalParent() is MainWindow mw) mw.Close();
        }
    }
}
