using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicCircuitEditor.Models.SerializebleElements
{
    public class SerializebleOutput : SerializebleLogicalElement
    {
        public bool SignalIn { get; set; }
        public bool IsConnected { get; set; }
    }
}
