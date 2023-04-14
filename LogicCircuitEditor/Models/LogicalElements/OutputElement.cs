namespace LogicCircuitEditor.Models.LogicalElements
{
    public class OutputElement : LogicalElement
    {
        private bool signalIn;
        private bool isConnected;

        public OutputElement()
        {
            SignalIn = false;
        }
        public bool SignalIn
        {
            get => signalIn;
            set => SetAndRaise(ref signalIn, value);
        }
        public bool IsConnected
        {
            get => isConnected;
            set => SetAndRaise(ref isConnected, value);
        }
    }
}