using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class GRP3 : TwoByteInstruction
    {
        public GRP3(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            /*
                REG 000: TEST R/M-8, IMM-8
                REG 001: NOT USED
                REG 002: NOT R/M-8
                REG 003: NEG R/M-8
                REG 004: MUL R/M-8
                REG 005: IMUL R/M-8
                REG 006: DIV R/M-8
                REG 007: IDIV R/M-8
            */

            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            int dest = 0;
            int result = 0;

            switch(secondByte.REG)
            {
                case 0x00: // TEST
                    {
                        if (wordSize == 0)
                        {
                            dest = Bus.NextImmediate();
                        }
                        else
                        {
                            dest = GetImmediateWord();
                        }
                        result = source & dest;
                        EU.CondReg.OverflowFlag = false;
                        EU.CondReg.CarryFlag = false;
                        EU.CondReg.CalcSignFlag(wordSize, result);
                        EU.CondReg.CalcZeroFlag(wordSize, result);
                        EU.CondReg.CalcParityFlag(result);
                        break;
                    }
                case 0x01: // not used
                    {
                        break;
                    }
                case 0x02: // NOT (no flags)
                    {
                        SaveToDestination(~source, 0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
                        break;
                    }
                case 0x03: // NEG (CF, ZF, SF, OF, PF, AF)
                    {
                        result = (~source) + 1;
                        SaveToDestination(result, 0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
                        EU.CondReg.CalcOverflowFlag(wordSize, 0, result);
                        EU.CondReg.CalcSignFlag(wordSize, result);
                        EU.CondReg.CalcZeroFlag(wordSize, result);
                        EU.CondReg.CalcAuxCarryFlag(source, dest);
                        EU.CondReg.CalcParityFlag(result);
                        EU.CondReg.CalcCarryFlag(wordSize, result); break;
                    }
                case 0x04: // MUL
                    {
                        if (wordSize == 0)
                        {
                            result = source * EU.Registers.AL;
                            EU.Registers.AX = (ushort)result;

                            EU.CondReg.CarryFlag = (EU.Registers.AH != 0);
                            EU.CondReg.OverflowFlag = EU.CondReg.CarryFlag;
                        }
                        else
                        {
                            result = source * EU.Registers.AX;
                            EU.Registers.DX = (ushort)(result >> 16);
                            EU.Registers.AX = (ushort)(result);

                            EU.CondReg.CarryFlag = (EU.Registers.DX != 0);
                            EU.CondReg.OverflowFlag = EU.CondReg.CarryFlag;
                        }
                        break;  
                    }
                case 0x05: // IMUL
                    {
                        if (wordSize == 0)
                        {
                            EU.Registers.AX = (ushort)(SignExtendByteToWord((byte)source) * SignExtendByteToWord(EU.Registers.AL));

                            if ((EU.Registers.AL & 0x80) == 0x80)
                                EU.CondReg.CarryFlag = (EU.Registers.AH != 0xff);
                            else
                                EU.CondReg.CarryFlag = (EU.Registers.AH != 0x00);

                            EU.CondReg.OverflowFlag = EU.CondReg.CarryFlag;
                        }
                        else
                        {
                            result = (int)(SignExtendByteToWord((byte)source) * SignExtendWordToDW(EU.Registers.AX));
                            EU.Registers.DX = (ushort)(result >> 16);
                            EU.Registers.AX = (ushort)(result);

                            if ((EU.Registers.AX & 0x8000) == 0x8000)
                                EU.CondReg.CarryFlag = (EU.Registers.DX != 0xffff);
                            else
                                EU.CondReg.CarryFlag = (EU.Registers.DX != 0x0000);

                            EU.CondReg.OverflowFlag = EU.CondReg.CarryFlag;
                        }
                        break;
                    }
                case 0x06: // DIV
                    {
                        if (wordSize == 0)
                        {
                            result = (byte)(EU.Registers.AX / source);
                            EU.Registers.AH = (byte)(EU.Registers.AX % source);
                            EU.Registers.AL = (byte)(result);
                        }
                        else
                        {
                            dest = (EU.Registers.DX << 16) | EU.Registers.AX;
                            EU.Registers.AX = (ushort)(dest / source);
                            EU.Registers.DX = (ushort)(dest % source);
                        }
                        break; 
                    }
                case 0x07: // IDIV
                    {
                        if (wordSize == 0)
                        {

                            ushort s1 = EU.Registers.AX;
                            ushort s2 = (ushort)source;

                            bool sign = ((s1 ^ s2) & 0x8000) == 0x8000;

                            if (s1 >= 0x8000)
                                s1 = (ushort)(~s1 + 1);
                            if (s2 >= 0x8000)
                                s2 = (ushort)(~s2 + 1);

                            ushort d1 = (ushort)(s1 / s2);
                            ushort d2 = (ushort)(s1 % s2);

                            if (sign)
                            {
                                d1 = (ushort)(~d1 + 1);
                                d2 = (ushort)(~d2 + 1);
                            }

                            EU.Registers.AL = (byte)d1;
                            EU.Registers.AH = (byte)d2;
                        }
                        else
                        {

                            uint dxax = (uint)((EU.Registers.DX << 16) | EU.Registers.AX);
                            uint divisor = SignExtendWordToDW((ushort)source);

                            bool sign = ((dxax ^ divisor) & 0x80000000) == 0x80000000;

                            if (dxax >= 0x80000000)
                                dxax = (uint)(~dxax + 1);

                            if (divisor >= 0x80000000)
                                divisor = (uint)(~divisor + 1);

                            uint d1 = (uint)(dxax / divisor);
                            uint d2 = (uint)(dxax % divisor);

                            if (sign)
                            {
                                d1 = (uint)((~d1 + 1) & 0xffff);
                                d2 = (uint)((~d2 + 1) & 0xffff);
                            }

                            EU.Registers.AX = (ushort)d1;
                            EU.Registers.DX = (ushort)d2;
                        }
                        break;
                    }
            }

        }

        public override long Clocks()
        {
            long clocks = 0;
            switch(secondByte.REG)
            {
                case 0x00: // TEST
                    {
                        break;
                    }
                case 0x02: // NOT
                    {
                        break;
                    }
                case 0x03: // NEG
                    {
                        if(secondByte.MOD == 0x03)
                        {
                            clocks = 3;
                        }
                        else
                        {
                            clocks = EffectiveAddressClocks + 16;
                        }
                        break;
                    }
                case 0x04: // MUL
                    {
                        if (wordSize == 0)
                        {
                            if (secondByte.MOD == 0x03)
                            {
                                clocks = 70;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 76;
                            }
                        }
                        else
                        {
                            if (secondByte.MOD == 0x03)
                            {
                                clocks = 118;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 124;
                            }
                        }
                        break;
                    }
                case 0x05: // IMUL
                    {
                        if (wordSize == 0)
                        {
                            if (secondByte.MOD == 0x03)
                            {
                                clocks = 80;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 86;
                            }
                        }
                        else
                        {
                            if (secondByte.MOD == 0x03)
                            {
                                clocks = 128;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 134;
                            }
                        }
                        break;
                    }
                case 0x06: // DIV
                    {
                        if( wordSize == 0)
                        {
                            if(secondByte.MOD == 0x03)
                            {
                                clocks = 80;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 86;
                            }
                        }
                        else
                        {
                            if (secondByte.MOD == 0x03)
                            {
                                clocks = 144;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 150;
                            }
                        }
                        break;
                    }
                case 0x07: // IDIV
                    {
                        if (wordSize == 0)
                        {
                            if (secondByte.MOD == 0x03)
                            {
                                clocks = 101;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 107;
                            }
                        }
                        else
                        {
                            if (secondByte.MOD == 0x03)
                            {
                                clocks = 165;
                            }
                            else
                            {
                                clocks = EffectiveAddressClocks + 171;
                            }
                        }
                        break;
                    }
            }
            return clocks;
        }

    }
}
