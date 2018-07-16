using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public interface IExecutionUnit
    {
        IBus Bus { get; }
        bool Halted { get; set; }
        Registers Registers { get; }
        ConditionalRegister CondReg { get; }
        long InstructionCount { get; } 
    }
}
