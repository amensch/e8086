using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086Disassembler
{
    public class Statistics
    {
        private long[] _opCount;
        public long InstructionCount { get; set; }

        public Statistics()
        {
            Reset();
        }

        public void Reset()
        {
            InstructionCount = 0;
            _opCount = new long[0x100];
            for( int i=0; i < 0x100; i++ )
            {
                _opCount[i] = 0;
            }
        }

        public void AddOpCode(byte op)
        {
            InstructionCount++;
            _opCount[op]++;
        }

        public string GetOpReport()
        {
            StringBuilder rpt = new StringBuilder();
            for (int i = 0; i < 0x100; i++)
            {
                rpt.AppendLine(string.Format("Op {0:X2} = {1}", i, _opCount[i]));
            }
            return rpt.ToString();
        }
    }
}
