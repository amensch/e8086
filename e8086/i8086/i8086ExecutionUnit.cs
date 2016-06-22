using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class i8086ExecutionUnit
    {
        /*

           EXECUTION UNIT (EU)                   BUS INTERFACE UNIT (BIU)
           --------------------                  ------------------------
            General Registers                      Segment Registers
                    |                             Instruction Pointer
                    |                                      |
                Operands                          Address Gen and Bus Control -----> external bus
                    |                                      |
                   ALU   <-----------------------  Instruction Queue
                    |  
                  Flags
        
           (From the 8086 Users Manual)
           The EU has no connection to the system bus -- the outside world.  It obtains instructions
           from a queue maintained by the BIU.  When an instruction requires access to memory or
           to a peripheral device, the EU requests the BIU to obtain or store the data.
           All addresses manipulated by the EU are 16 bits wide.  The BIU performs address relocation
           to give the EU access to the full megabyte of memory space.
           
           (From the 8086 Users Manual - paraphrased)
           The BIU performs all bus operations for the EU.   Data is transferred between the CPU
           and memory or I/O devices.

         */

        // Table of OpCodes
        private OpCodeTable _opTable = new OpCodeTable();

        // General Registers: AX, BX, CX, DX and SP, BP, SI, DI
        private i8086Registers _reg = new i8086Registers();

        // Flags
        private i8086ConditionalRegister _creg = new i8086ConditionalRegister();

        // Bus Interface Unit
        private i8086BusInterfaceUnit _bus;

        public i8086ExecutionUnit(i8086BusInterfaceUnit bus)
        {
            _bus = bus;
            InitOpCodeTable();
        }

        private void InitOpCodeTable()
        {
            _opTable[0x88] = new OpCodeRecord(0x88, 10, ExecuteMOV_88);
            _opTable[0x89] = new OpCodeRecord(0x88, 10, ExecuteMOV_89);
            _opTable[0x8a] = new OpCodeRecord(0x88, 10, ExecuteMOV_90);
            _opTable[0x8b] = new OpCodeRecord(0x88, 10, ExecuteMOV_91);
        }


        private int ExecuteMOV_88()
        {
            return 2;
        }

        private int ExecuteMOV_89()
        {
            return 2;
        }
        private int ExecuteMOV_90()
        {
            return 2;
        }
        private int ExecuteMOV_91()
        {
            return 2;
        }

        public void AdjustAfterAddition()
        {
            if( _reg.AL > 9 || _creg.AuxCarryFlag )
            {
                _reg.AL += 6;
                _reg.AH += 1;
                _creg.AuxCarryFlag = true;
                _creg.CarryFlag = true;
            }
            else
            {
                _creg.AuxCarryFlag = false;
                _creg.CarryFlag = false;
            }
            // always clear high nibble of AL
            _reg.AL = (byte)(_reg.AL & 0x0f);
        }

    }
}
