using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class RotateAndShift : TwoByteInstruction
    {
        public RotateAndShift(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        /*
            This is Group 2 instructions 
         
            D0:  <op> reg/mem-8, 1
            D1:  <op> reg/mem-16, 1
            D2:  <op> reg/mem-8, CL
            D3:  <op> reg/mem-16, CL

            REG 000: ROL - rotate left
            REG 001: ROR - rotate right 
            REG 002: RCL - rotate through carry left  (CF is low order bit)
            REG 003: RCR - rotate through carry right (CF is low order bit)
            REG 004: SAL/SHL - shift left (zero padded) (OF is cleared if sign bit is unchanged)
            REG 005: SHR - shift right (zero padded) (OF is cleared if sign bit is unchanged)
            REG 006: NOT USED
            REG 007: SAR - shift arithmetic right

            Shifts affect PF, SF, ZF.  CF is the last bit shifted out.
                OF is defined only for a single bit shift.

            Rotate affects CF and OF only.
        */

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData();
            switch (secondByte.REG)
            {
                case 0x00:
                    {
                        RotateLeft(source, secondByte.MOD, secondByte.RM, false, false);
                        break;
                    }
                case 0x01:
                    {
                        RotateRight(source, secondByte.MOD, secondByte.RM, false, false, false);
                        break;
                    }
                case 0x02:
                    {
                        RotateLeft(source, secondByte.MOD, secondByte.RM, true, false);
                        break;
                    }
                case 0x03:
                    {
                        RotateRight(source, secondByte.MOD, secondByte.RM, true, false, false);
                        break;
                    }
                case 0x04:
                    {
                        RotateLeft(source, secondByte.MOD, secondByte.RM, false, true);
                        break;
                    }
                case 0x05:
                    {
                        RotateRight(source, secondByte.MOD, secondByte.RM, false, true, false);
                        break;
                    }
                case 0x07:
                    {
                        RotateRight(source, secondByte.MOD, secondByte.RM, false, true, true);
                        break;
                    }
            }
        }

        public override long Clocks()
        {
            // the direction bit indicates the source
            // direction = 0 shift or rotate by 1
            // direction = 1 shift or rotate by the value in CL

            if(direction == 0)
            {
                if(secondByte.MOD == 0x03)
                {
                    return 2;
                }
                else
                {
                    return EffectiveAddressClocks + 15;
                }
            }
            else
            {
                if(secondByte.MOD == 0x03)
                {
                    return 8 + (4 * GetSourceData());
                }
                else
                {
                    return EffectiveAddressClocks + 20 + (4 * GetSourceData());
                }
            }
        }
        protected virtual int GetSourceData()
        {
            return 1;
        }

        private void RotateLeft(int source, byte mod, byte rm, bool through_carry, bool shift_only)
        {
            if (wordSize == 0)
                RotateByteLeft(source, mod, rm, through_carry, shift_only);
            else
                RotateWordLeft(source, mod, rm, through_carry, shift_only);
        }

        private void RotateRight(int source, byte mod, byte rm, bool through_carry, bool shift_only, bool arithmetic_shift)
        {
            if (wordSize == 0)
                RotateByteRight(source, mod, rm, through_carry, shift_only, arithmetic_shift);
            else
                RotateWordRight(source, mod, rm, through_carry, shift_only, arithmetic_shift);
        }

        private void RotateByteLeft(int source, byte mod, byte rm, bool through_carry, bool shift_only)
        {
            int original = 0;
            byte result = 0;
            bool old_CF;
            int offset = 0;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x03:
                    {
                        original = EU.Registers.GetRegisterValue(wordSize, rm);
                        break;
                    }
            }

            // preserve the original value
            result = (byte)(original & 0xff);

            // perform the rotation or shift
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = EU.CondReg.CarryFlag;

                // carry bit equal to high bit
                EU.CondReg.CarryFlag = ((result & 0x80) == 0x80);

                // shift left
                result = (byte)((result << 1) & 0xff);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes low bit
                    // otherwise the original high bit (which is now the CF) becomes low bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (byte)(result | 0x01);
                    }
                    else
                    {
                        if (EU.CondReg.CarryFlag)
                            result = (byte)(result | 0x01);
                    }
                }
            }

            // save the result
            SaveToDestination(result, 0, 0, mod, 0, rm);

            // if the operand is 1 then the overflow flag is defined
            if (source == 1)
            {
                // when shifting 1, if the two high order bits changed, set OF
                if (shift_only)
                {
                    EU.CondReg.OverflowFlag = ((original & 0xc0) != (result & 0xc0));
                }
                else
                {
                    // when rotating 1, if the sign changes as a result of the rotate, set OF
                    EU.CondReg.OverflowFlag = ((original ^ result) & 0x80) == 0x80;
                }
            }
        }

        private void RotateWordLeft(int source, byte mod, byte rm, bool through_carry, bool shift_only)
        {
            int original = 0;
            int result = 0;
            int offset = 0;
            bool old_CF;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x03:
                    {
                        original = EU.Registers.GetRegisterValue(wordSize, rm);
                        break;
                    }
            }

            // preserve the original value
            result = original;

            // perform the rotation
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = EU.CondReg.CarryFlag;

                // carry bit equal to high bit
                EU.CondReg.CarryFlag = ((result & 0x8000) == 0x8000);

                // shift left
                result = (ushort)(result << 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes low bit
                    // otherwise the original high bit (which is now the CF) becomes low bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (ushort)(result | 0x0001);
                    }
                    else
                    {
                        if (EU.CondReg.CarryFlag)
                            result = (ushort)(result | 0x0001);
                    }
                }
            }

            // save result
            SaveToDestination(result, 0, 1, mod, 0, rm);

            // if the operand is 1 then the overflow flag is defined
            if (source == 1)
            {
                // when shifting 1, if the two high order bits changed, set OF
                if (shift_only)
                {
                    EU.CondReg.OverflowFlag = ((original & 0xc000) != (result & 0xc000));
                }
                else
                {
                    // when rotating 1, if the sign changes as a result of the rotate, set OF
                    EU.CondReg.OverflowFlag = ((original ^ result) & 0x8000) == 0x8000;
                }
            }
        }

        private void RotateByteRight(int source, byte mod, byte rm, bool through_carry, bool shift_only, bool arithmetic_shift)
        {
            int original = 0;
            int result = 0;
            int offset = 0;
            bool old_CF = false;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x03:
                    {
                        original = EU.Registers.GetRegisterValue(wordSize, rm);
                        break;
                    }
            }

            // preserve the original value
            result = original;

            // perform the rotation
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = EU.CondReg.CarryFlag;

                // carry bit equal to low bit
                EU.CondReg.CarryFlag = ((result & 0x01) == 0x01);

                // shift right
                result = (byte)(result >> 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes high bit
                    // otherwise the original low bit (which is now the CF) becomes high bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (byte)(result | 0x80);
                    }
                    else
                    {
                        if (EU.CondReg.CarryFlag)
                            result = (byte)(result | 0x80);
                    }
                }

                // if arithmetic shift then the sign bit retains its original value
                // set SF, ZF, PF
                if (arithmetic_shift)
                {
                    result = (byte)(result | (byte)(original & 0x80));
                    EU.CondReg.CalcSignFlag(0, result);
                    EU.CondReg.CalcZeroFlag(0, result);
                    EU.CondReg.CalcParityFlag(result);
                }
            }

            // save the result
            SaveToDestination(result, 0, 0, mod, 0, rm);

            // overflow flag - only calculated if count is 1
            if (source == 1)
            {
                // arithmetic shift always clears OF
                if (arithmetic_shift)
                {
                    EU.CondReg.OverflowFlag = false;
                }
                // if shift, set OF if the sign has changed
                else if (shift_only)
                {
                    EU.CondReg.OverflowFlag = ((original ^ result) & 0x80) == 0x80;
                }
                // if rotate through carry, set OF if high order bit and carry flags have changed
                else if (!shift_only & through_carry)
                {
                    EU.CondReg.OverflowFlag = (((original ^ result) & 0x80) == 0x80) & (old_CF == EU.CondReg.CarryFlag);
                }
            }
        }

        private void RotateWordRight(int source, byte mod, byte rm, bool through_carry, bool shift_only, bool arithmetic_shift)
        {
            int original = 0;
            int result = 0;
            int offset = 0;
            bool old_CF = false;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetData(wordSize, offset);
                        break;
                    }
                case 0x03:
                    {
                        original = EU.Registers.GetRegisterValue(wordSize, rm);
                        break;
                    }
            }

            // preserve the original value
            result = original;

            // perform the rotation
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = EU.CondReg.CarryFlag;

                // carry bit equal to low bit
                EU.CondReg.CarryFlag = ((result & 0x0001) == 0x0001);

                // shift right
                result = (ushort)(result >> 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes high bit
                    // otherwise the original low bit (which is now the CF) becomes high bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (ushort)(result | 0x8000);
                    }
                    else
                    {
                        if (EU.CondReg.CarryFlag)
                            result = (ushort)(result | 0x8000);
                    }
                }

                // if arithmetic shift then the sign bit retains its original value
                if (arithmetic_shift)
                {
                    result = (byte)(result | (byte)(original & 0x80));
                    EU.CondReg.CalcSignFlag(1, result);
                    EU.CondReg.CalcZeroFlag(1, result);
                    EU.CondReg.CalcParityFlag(result);
                }
            }

            // save the result
            SaveToDestination(result, 0, 1, mod, 0, rm);

            // overflow flag - only calculated if count is 1
            if (source == 1)
            {
                // arithmetic shift always clears OF
                if (arithmetic_shift)
                {
                    EU.CondReg.OverflowFlag = false;
                }
                // if shift, set OF if the sign has changed
                else if (shift_only)
                {
                    EU.CondReg.OverflowFlag = ((original ^ result) & 0x8000) == 0x8000;
                }
                // if rotate through carry, set OF if high order bit and carry flags have changed
                else if (!shift_only & through_carry)
                {
                    EU.CondReg.OverflowFlag = (((original ^ result) & 0x8000) == 0x8000) & (old_CF == EU.CondReg.CarryFlag);
                }
            }
        }



    }
}
