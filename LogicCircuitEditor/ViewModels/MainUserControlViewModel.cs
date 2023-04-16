using DynamicData;
using LogicCircuitEditor.Models;
using LogicCircuitEditor.Models.LogicalElements;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace LogicCircuitEditor.ViewModels
{
    public class MainUserControlViewModel : ViewModelBase
    {
        private ObservableCollection<Element> _elements;
        private Project _project;
        private Stack<Connector> _connectWithInputElement;
        private int _index;
        private bool isInput = false;
        private bool isOutput = false;
        private bool isNotGate = false;
        private bool isAndGate = false;
        private bool isOrGate = false;
        private bool isXorGate = false;
        private bool isDecoderGate = false;
        private bool changeElements = false;
        public MainUserControlViewModel()
        {
            Elements = new ObservableCollection<Element>();
            Project = new Project();
            IsInput = true;
            Index = 0;
            DeleteFocusedElement = ReactiveCommand.Create(() =>
            {
                for (int i = 0; i < Elements.Count; ++i)
                {
                    if (Elements[i].FocusOnElement == true)
                    {
                        if (Elements[i] is Connector connector) connector.Dispose();
                        if (Elements[i] is LogicalElement)
                        {
                            for (int j = i + 1; j < Elements.Count;)
                            {
                                if (Elements[j] is Connector con)
                                {
                                    if (con.FirstElement == Elements[i] || con.SecondElement == Elements[i])
                                    {
                                        con.Dispose();
                                        Elements.RemoveAt(j);
                                    }
                                    else j++;
                                }
                                else j++;
                            }
                        }
                        Elements.RemoveAt(i);
                        CalcInputConnectors();
                        SignalBypass();
                        break;
                    }
                }
            });
            AddScheme = ReactiveCommand.Create(() =>
            {
                Project.Schemes.Add(new Scheme());
                Index = Project.Schemes.Count - 1;
            });
            DeleteScheme = ReactiveCommand.Create(() =>
            {
                if (Project.Schemes.Count > 1)
                {
                    int index = _index;
                    if (_index != 0) { Index = 0; Project.Schemes.RemoveAt(index); }
                    else { for (int i = 0; i < Project.Schemes.Count - 1; ++i) { Project.Schemes[i].Name = Project.Schemes[i + 1].Name; Project.Schemes[i].Elements = Project.Schemes[i + 1].Elements; } changeElements = true; Index = 0; changeElements = false; Project.Schemes.RemoveAt(Project.Schemes.Count - 1); }
                }
            });
            CreateProject = ReactiveCommand.Create(() =>
            {
                Index = 0;
                changeElements = true;
                Project = new Project();
                changeElements = false;
                Index = 0;
            });
        }
        public Project Project
        {
            get => _project;
            set => this.RaiseAndSetIfChanged(ref _project, value);
        }

        public ObservableCollection<Element> Elements
        {
            get => _elements;
            set
            {
                this.RaiseAndSetIfChanged(ref _elements, value);
                CalcInputConnectors();
            }
        }

        public Stack<Connector> ConnectWithInputElement
        {
            get => _connectWithInputElement;
            set => _connectWithInputElement = value;
        }

        public int Index
        {
            get => _index;
            set
            {
                if (!changeElements) Project.Schemes[_index].Elements = Elements;
                if (value != -1) this.RaiseAndSetIfChanged(ref _index, value);
                Elements = Project.Schemes[_index].Elements;
            }
        }
        public bool IsInput
        {
            get => isInput;
            set => this.RaiseAndSetIfChanged(ref isInput, value);
        }
        public bool IsOutput
        {
            get => isOutput;
            set => this.RaiseAndSetIfChanged(ref isOutput, value);
        }
        public bool IsNotGate
        {
            get => isNotGate;
            set => this.RaiseAndSetIfChanged(ref isNotGate, value);
        }
        public bool IsAndGate
        {
            get => isAndGate;
            set => this.RaiseAndSetIfChanged(ref isAndGate, value);
        }
        public bool IsOrGate
        {
            get => isOrGate;
            set => this.RaiseAndSetIfChanged(ref isOrGate, value);
        }
        public bool IsXorGate
        {
            get => isXorGate;
            set => this.RaiseAndSetIfChanged(ref isXorGate, value);
        }
        public bool IsDecoderGate
        {
            get => isDecoderGate;
            set => this.RaiseAndSetIfChanged(ref isDecoderGate, value);
        }

        public bool ChangeElements
        {
            get => changeElements;
            set => changeElements = value;
        }

        private void CalcInputConnectors()
        {
            ConnectWithInputElement = new Stack<Connector>();
            foreach (Element element in Elements)
            {
                if (element is Connector con)
                {
                    if (con.FirstElement is InputElement) ConnectWithInputElement.Push(con);
                }
            }
        }

        public void SignalBypass()
        {
            Stack<Connector> con = new Stack<Connector>(new Stack<Connector>(ConnectWithInputElement));
            while (con.Count != 0)
            {
                Connector connector = con.Pop();
                if (connector.FirstElement is InputElement inputElement)
                {
                    if (connector.SecondElement is OutputElement outputElement) outputElement.SignalIn = inputElement.SignalOut;
                    if (connector.SecondElement is NotGate notGate)
                    {
                        notGate.Input = inputElement.SignalOut;
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                                if (conect.FirstElement == notGate) con.Push(conect);
                        }
                    }
                    if (connector.SecondElement is TwoInputsGate two)
                    {
                        if (connector.ConnectToFirstInput) two.Input1 = inputElement.SignalOut;
                        else two.Input2 = inputElement.SignalOut;
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                                if (conect.FirstElement == two) con.Push(conect);
                        }
                    }
                }
                if (connector.FirstElement is DecoderGate decoderGate)
                {
                    if (connector.ConnectFromFirstInput)
                    {
                        if (connector.SecondElement is OutputElement outputElement) outputElement.SignalIn = decoderGate.Output();
                        if (connector.SecondElement is NotGate notGate)
                        {
                            notGate.Input = decoderGate.Output();
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                    if (conect.FirstElement == notGate) con.Push(conect);
                            }
                        }
                        if (connector.SecondElement is TwoInputsGate two)
                        {
                            if (connector.ConnectToFirstInput) two.Input1 = decoderGate.Output();
                            else two.Input2 = decoderGate.Output();
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                    if (conect.FirstElement == two) con.Push(conect);
                            }
                        }
                    }
                    else
                    {
                        if (connector.SecondElement is OutputElement outputElement) outputElement.SignalIn = decoderGate.Output2();
                        if (connector.SecondElement is NotGate notGate)
                        {
                            notGate.Input = decoderGate.Output2();
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                    if (conect.FirstElement == notGate) con.Push(conect);
                            }
                        }
                        if (connector.SecondElement is TwoInputsGate two)
                        {
                            if (connector.ConnectToFirstInput) two.Input1 = decoderGate.Output2();
                            else two.Input2 = decoderGate.Output2();
                            foreach (Element element in Elements)
                            {
                                if (element is Connector conect)
                                    if (conect.FirstElement == two) con.Push(conect);
                            }
                        }
                    }
                }
                else if (connector.FirstElement is Gate gate)
                {
                    if (connector.SecondElement is OutputElement outputElement) outputElement.SignalIn = gate.Output();
                    if (connector.SecondElement is NotGate notGate)
                    {
                        notGate.Input = gate.Output();
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                                if (conect.FirstElement == notGate) con.Push(conect);
                        }
                    }
                    if (connector.SecondElement is TwoInputsGate two)
                    {
                        if (connector.ConnectToFirstInput) two.Input1 = gate.Output();
                        else two.Input2 = gate.Output();
                        foreach (Element element in Elements)
                        {
                            if (element is Connector conect)
                                if (conect.FirstElement == two) con.Push(conect);
                        }
                    }
                }
            }
        }

        public ReactiveCommand<Unit, Unit> DeleteFocusedElement { get; }
        public ReactiveCommand<Unit, Unit> AddScheme { get; }
        public ReactiveCommand<Unit, Unit> DeleteScheme { get; }
        public ReactiveCommand<Unit, Unit> CreateProject { get; }
    }
}