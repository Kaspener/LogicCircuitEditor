using Avalonia;
using DynamicData;
using LogicCircuitEditor.Models.LogicalElements;
using LogicCircuitEditor.Models.SerializebleElements;
using System.Collections.ObjectModel;
using System.IO;
using YamlDotNet.Serialization;
using InputElement = LogicCircuitEditor.Models.LogicalElements.InputElement;

namespace LogicCircuitEditor.Models
{
    public static class Serializer
    {
        public static void Save(string path, Project data)
        {
            Project project = ToSerializeble(data);
            var serializer = new SerializerBuilder()
                .WithTagMapping("!scheme", typeof(Scheme))
                .WithTagMapping("!connector", typeof(SerializebleConnector))
                .WithTagMapping("!input", typeof(SerializebleInput))
                .WithTagMapping("!output", typeof(SerializebleOutput))
                .WithTagMapping("!twoinputs", typeof(SerializebleTwoInputElements))
                .WithTagMapping("!not", typeof(SerializebleNot))
                .Build();
            var yaml = serializer.Serialize(project);
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(yaml);
            }
        }

        public static Project Load(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithTagMapping("!scheme", typeof(Scheme))
                .WithTagMapping("!connector", typeof(SerializebleConnector))
                .WithTagMapping("!input", typeof(SerializebleInput))
                .WithTagMapping("!output", typeof(SerializebleOutput))
                .WithTagMapping("!twoinputs", typeof(SerializebleTwoInputElements))
                .WithTagMapping("!not", typeof(SerializebleNot))
                .Build();
            string input;
            using (StreamReader reader = new StreamReader(path))
            {
                input = reader.ReadToEnd();
            }
            var ser = deserializer.Deserialize<Project>(input);
            return FromSerializeble(ser);
        }

        public static Project ToSerializeble(Project data)
        {
            Project new_project = new Project { Name = data.Name };
            var schemes = data.Schemes;
            ObservableCollection<Scheme> new_schemes = new ObservableCollection<Scheme>();
            foreach (Scheme scheme in schemes)
            {
                ObservableCollection<Element> new_elements = new ObservableCollection<Element>();
                var elements = scheme.Elements;
                foreach (Element element in elements)
                {
                    if (element is Connector connector)
                    {
                        new_elements.Add(new SerializebleConnector
                        {
                            ID = connector.ID,
                            FocusOnElement = connector.FocusOnElement,
                            ConnectFromFirstInput = connector.ConnectFromFirstInput,
                            ConnectToFirstInput = connector.ConnectToFirstInput,
                            ReverseConnection = connector.ReverseConnection,
                            StartPoint = connector.StartPoint.ToString(),
                            EndPoint = connector.EndPoint.ToString(),
                            FirstElement = connector.FirstElement.ID,
                            SecondElement = connector.SecondElement.ID
                        });
                        continue;
                    }
                    if (element is InputElement input)
                    {
                        new_elements.Add(new SerializebleInput { ID = input.ID, FocusOnElement = input.FocusOnElement, SignalOut = input.SignalOut, StartPoint = input.StartPoint.ToString() });
                        continue;
                    }
                    if (element is OutputElement output)
                    {
                        new_elements.Add(new SerializebleOutput { ID = output.ID, FocusOnElement = output.FocusOnElement, IsConnected = output.IsConnected, SignalIn = output.SignalIn, StartPoint = output.StartPoint.ToString() });
                        continue;
                    }
                    if (element is TwoInputsGate two)
                    {
                        var elem = new SerializebleTwoInputElements { ID = two.ID, FocusOnElement = two.FocusOnElement, Input1 = two.Input1, Input2 = two.Input2, IsConnectedInput1 = two.IsConnectedInput1, IsConnectedInput2 = two.IsConnectedInput2, StartPoint = two.StartPoint.ToString() };
                        if (two is AndGate) elem.Type = "AndGate";
                        if (two is OrGate) elem.Type = "OrGate";
                        if (two is XorGate) elem.Type = "XorGate";
                        if (two is DecoderGate) elem.Type = "DecoderGate";
                        new_elements.Add(elem);
                        continue;
                    }

                    if (element is NotGate not)
                    {
                        new_elements.Add(new SerializebleNot { ID = not.ID, FocusOnElement = not.FocusOnElement, Input = not.Input, IsConnected = not.IsConnected, StartPoint = not.StartPoint.ToString() });
                        continue;
                    }
                }
                new_schemes.Add(new Scheme { Name = scheme.Name, Elements = new_elements });
            }
            new_project.Schemes = new_schemes;
            return new_project;
        }

        public static Project FromSerializeble(Project data)
        {
            Project new_project = new Project { Name = data.Name };
            var schemes = data.Schemes;
            ObservableCollection<Scheme> new_schemes = new ObservableCollection<Scheme>();
            foreach (Scheme scheme in schemes)
            {
                ObservableCollection<Element> new_elements = new ObservableCollection<Element>();
                var elements = scheme.Elements;
                foreach (Element element in elements)
                {
                    if (element is SerializebleConnector connector)
                    {
                        Connector new_connector = new Connector
                        {
                            ID = connector.ID,
                            FocusOnElement = connector.FocusOnElement,
                            ConnectFromFirstInput = connector.ConnectFromFirstInput,
                            ConnectToFirstInput = connector.ConnectToFirstInput,
                            ReverseConnection = connector.ReverseConnection,
                            StartPoint = Point.Parse(connector.StartPoint),
                            EndPoint = Point.Parse(connector.EndPoint)
                        };
                        foreach (Element el in new_elements)
                        {
                            if (el is LogicalElement log)
                            {
                                if (log.ID == connector.FirstElement) { new_connector.FirstElement = log; break; }
                            }
                        }
                        foreach (Element el in new_elements)
                        {
                            if (el is LogicalElement log)
                            {
                                if (log.ID == connector.SecondElement) { new_connector.SecondElement = log; break; }
                            }
                        }
                        new_elements.Add(new_connector);
                        continue;
                    }
                    if (element is SerializebleInput input)
                    {
                        new_elements.Add(new InputElement { ID = input.ID, FocusOnElement = input.FocusOnElement, SignalOut = input.SignalOut, StartPoint = Point.Parse(input.StartPoint) });
                        continue;
                    }
                    if (element is SerializebleOutput output)
                    {
                        new_elements.Add(new OutputElement { ID = output.ID, FocusOnElement = output.FocusOnElement, IsConnected = output.IsConnected, SignalIn = output.SignalIn, StartPoint = Point.Parse(output.StartPoint) });
                        continue;
                    }
                    if (element is SerializebleNot not)
                    {
                        new_elements.Add(new NotGate { ID = not.ID, FocusOnElement = not.FocusOnElement, Input = not.Input, StartPoint = Point.Parse(not.StartPoint), IsConnected = not.IsConnected });
                        continue;
                    }
                    if (element is SerializebleTwoInputElements two)
                    {
                        if (two.Type == "AndGate")
                        {
                            new_elements.Add(new AndGate { ID = two.ID, FocusOnElement = two.FocusOnElement, Input1 = two.Input1, Input2 = two.Input2, IsConnectedInput1 = two.IsConnectedInput1, IsConnectedInput2 = two.IsConnectedInput2, StartPoint = Point.Parse(two.StartPoint) });
                            continue;
                        }
                        if (two.Type == "OrGate")
                        {
                            new_elements.Add(new OrGate { ID = two.ID, FocusOnElement = two.FocusOnElement, Input1 = two.Input1, Input2 = two.Input2, IsConnectedInput1 = two.IsConnectedInput1, IsConnectedInput2 = two.IsConnectedInput2, StartPoint = Point.Parse(two.StartPoint) });
                            continue;
                        }
                        if (two.Type == "XorGate")
                        {
                            new_elements.Add(new XorGate { ID = two.ID, FocusOnElement = two.FocusOnElement, Input1 = two.Input1, Input2 = two.Input2, IsConnectedInput1 = two.IsConnectedInput1, IsConnectedInput2 = two.IsConnectedInput2, StartPoint = Point.Parse(two.StartPoint) });
                            continue;
                        }
                        if (two.Type == "DecoderGate")
                        {
                            new_elements.Add(new DecoderGate { ID = two.ID, FocusOnElement = two.FocusOnElement, Input1 = two.Input1, Input2 = two.Input2, IsConnectedInput1 = two.IsConnectedInput1, IsConnectedInput2 = two.IsConnectedInput2, StartPoint = Point.Parse(two.StartPoint) });
                            continue;
                        }
                    }
                }
                new_schemes.Add(new Scheme { Name = scheme.Name, Elements = new_elements });
            }
            new_project.Schemes = new_schemes;
            return new_project;
        }


    }

}