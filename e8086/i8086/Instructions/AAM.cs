using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// AAM: Ascii Adjust Multiply
    /// </summary>
    public class AAM : Instruction
    {
        public AAM(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            EU.Registers.AH = (byte)(EU.Registers.AL / 10);
            EU.Registers.AL = (byte)(EU.Registers.AL % 10);
            EU.CondReg.CalcParityFlag(EU.Registers.AX);
            EU.CondReg.CalcSignFlag(1, EU.Registers.AX);
            EU.CondReg.CalcZeroFlag(1, EU.Registers.AX);
        }

    }
}
