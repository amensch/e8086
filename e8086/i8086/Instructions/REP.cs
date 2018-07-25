using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// REP - REPE - REPZ
    /// REP is used with MOVS and STOS and is interpreted as "repeat while not end of string"
    /// This mean repeat while CX is not 0.
    /// REPE and REPZ are used with CMPS and SCAS. ZF must be set prior to use.
    /// </summary>
    internal class REP : Instruction
    {
        public REP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            EU.RepeatMode = RepeatModeEnum.REPNZ;
        }

    }
}
