using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class OpCodeTable
    {
        // Table of OpCodes
        private OpCodeRecord[] _opCodeTable = new OpCodeRecord[0x100];

        public OpCodeTable()
        {
            // initialize the entire table with new records
            for (UInt16 ii = 0; ii < _opCodeTable.Length; ii++)
            {
                _opCodeTable[ii] = new OpCodeRecord(ii, 0, OpCodeNotImplemented);
            }
        }

        public OpCodeRecord this[int index]
        {
            get
            {
                return _opCodeTable[index];
            }
            set
            {
                _opCodeTable[index] = value;
            }
        }

        private int OpCodeNotImplemented()
        {
            throw new NotImplementedException("Instruction not implemented");
        }


    }
}
