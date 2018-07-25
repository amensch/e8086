using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// AAD: Ascii Adjust Divide
    /// </summary>
    internal class AAD : Instruction
    {
        public AAD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            EU.Registers.AL += (byte)(EU.Registers.AH * 10);
            EU.Registers.AH = 0;
            EU.CondReg.CalcParityFlag(EU.Registers.AX);
            EU.CondReg.CalcSignFlag(1, EU.Registers.AX);
            EU.CondReg.CalcZeroFlag(1, EU.Registers.AX);
        }

    }
}
