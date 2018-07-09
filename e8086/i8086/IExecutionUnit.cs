using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public interface IExecutionUnit
    {
        IBus Bus { get; set; }
        bool Halted { get; set; }
        i8086Registers Registers { get; }
        i8086ConditionalRegister CondReg { get; }
        long InstructionCount { get; set; } 
    }
}
