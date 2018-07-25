using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// REPNE, REPNZ are 2 mnemonics for the same instruction
    /// Repeat while not zero -- stop repeating when zero flag = true
    /// </summary>
    internal class REPNZ : Instruction
    {
        public REPNZ(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            EU.RepeatMode = RepeatModeEnum.REPNZ;
        }

    }
}
