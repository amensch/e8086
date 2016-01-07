using System;
using System.Text;
using KDS.Loader;

namespace KDS.e8086
{
    public class Disassemble8086 : ICodeDisassembler
    {

        /*

            0000 0110 = 0x06

        */

        // RegField[reg][w]
        public static string[,] RegField = new string[,]
        {
            {"al","ax" },
            {"cl","cx" },
            {"dl","dx" },
            {"bl","bx" },
            {"ah","sp" },
            {"ch","bp" },
            {"dh","si" },
            {"bh","di" },
        };

        public static string[] RMTable1 = new string[]
        {
            "[bx+si]","[bx+di]","[bp+si]","[bp+di]","si","di","direct","bx"
        };

        public static string[] RMTable2 = new string[]
        {
            "bx+si","bx+di","bp+si","bp+di","si","di","bp","bx"
        };

        public static string[] SegRegTable = new string[]
        {
            "es","cs","ss","ds"
        };

        public string Disassemble(ICodeLoader loader)
        {
            throw new NotImplementedException();
        }

        public string Disassemble(ICodeLoader loader, UInt16 startingAddress)
        {
            byte[] buffer = loader.LoadData();
            StringBuilder output = new StringBuilder();

            UInt32 pc = 0;
            string line;

            while (pc < buffer.Length)
            {
                pc += DisassembleNext(buffer, pc, startingAddress, out line);
                output.AppendLine(line);
            }

            return output.ToString();
        }

        public static UInt32 DisassembleNext(byte[] buffer, UInt32 pc, UInt16 startingAddress, out string output)
        {
            if (buffer.Length <= pc)
                throw new IndexOutOfRangeException("Index pc exceeds the bounds of the buffer");

            byte op = buffer[pc];
            output = "";
            UInt32 bytes_read = 1;

            if (i8086Table.opCodes[op].num_operands == -1)
                throw new NotImplementedException("The instruction " + op.ToString("X2") + " is not implemented");

            if (i8086Table.opCodes[op].num_operands == 0)
            {
                output = i8086Table.opCodes[op].dasm_format;
            }
            else if (i8086Table.opCodes[op].num_operands == 1)
            {
                throw new NotImplementedException("Instruction " + op.ToString("X2") + " has 1 operand and is not implemented");
            }
            else if (i8086Table.opCodes[op].num_operands == 2)
            {
                OpCodeTable oc = i8086Table.opCodes[op];
                byte addr_mode = buffer[pc + 1];
                int w = IsOperSizeByte(op) ? 1 : 0;

                string operand1;
                string operand2;

                if (oc.arg1_mode == "R/M")
                {

                }
            }
            

            #region switch that might not be needed
            //switch(op)
            //{

            //    #region "0x00-0x0f"
            //    // ADD instructions
            //    case 0x00:
            //        {
                        
            //            break;
            //        }
            //    case 0x01:
            //    case 0x02:
            //    case 0x03:
            //    case 0x04:
            //    case 0x05:
            //        {



            //            break;
            //        }

            //    case 0x06:
            //        {
            //            output = "push es";
            //            break;
            //        }

            //    case 0x07:
            //        {
            //            output = "pop es";
            //            break;
            //        }

            //    // OR instructions
            //    case 0x08: case 0x09: case 0x0a: case 0x0b: case 0x0c: case 0x0d:
            //        {
            //            break;
            //        }

            //    case 0x0e:
            //        {
            //            output = "push cs";
            //            break;
            //        }

            //    case 0x0f:
            //        {
            //            output = "pop cs";
            //            break;
            //        }
            //    #endregion

            //    #region "0x10-0x1f"
            //    // ADC instructions
            //    case 0x10: case 0x11: case 0x12: case 0x13: case 0x14: case 0x15:
            //        {
            //            break;
            //        }

            //    case 0x16:
            //        {
            //            output = "es:";
            //            break;
            //        }

            //    case 0x17:
            //        {
            //            output = "daa";
            //            break;
            //        }

            //    // SBB instructions
            //    case 0x18: case 0x19: case 0x1a: case 0x1b: case 0x1c: case 0x1d:
            //        {
            //            break;
            //        }

            //    case 0x1e:
            //        {
            //            output = "cs:";
            //            break;
            //        }

            //    case 0x1f:
            //        {
            //            output = "das";
            //            break;
            //        }
            //    #endregion

            //    #region "0x20-0x2f"
            //    // AND instructions
            //    case 0x20: case 0x21: case 0x22: case 0x23: case 0x24: case 0x25:
            //        {
            //            break;
            //        }

            //    case 0x26:
            //        {
            //            output = "push ss";
            //            break;
            //        }

            //    case 0x27:
            //        {
            //            output = "pop ss";
            //            break;
            //        }

            //    // SUB instructions
            //    case 0x28: case 0x29: case 0x2a: case 0x2b: case 0x2c: case 0x2d:
            //        {
            //            break;
            //        }

            //    case 0x2e:
            //        {
            //            output = "push ds";
            //            break;
            //        }

            //    case 0x2f:
            //        {
            //            output = "pop ds";
            //            break;
            //        }
            //    #endregion

            //    #region "0x30-0x3f"
            //    // XOR instructions
            //    case 0x30: case 0x31: case 0x32: case 0x33: case 0x34: case 0x35:
            //        {
            //            break;
            //        }

            //    case 0x36:
            //        {
            //            output = "ss:";
            //            break;
            //        }

            //    case 0x37:
            //        {
            //            output = "aaa";
            //            break;
            //        }

            //    // CMP instructions
            //    case 0x38: case 0x39: case 0x3a: case 0x3b: case 0x3c: case 0x3d:
            //        {
            //            break;
            //        }

            //    case 0x3e:
            //        {
            //            output = "ds:";
            //            break;
            //        }

            //    case 0x3f:
            //        {
            //            output = "aas";
            //            break;
            //        }
            //    #endregion

            //    #region "0x40-0x4f"
            //    // INC instructions
            //    case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x46: case 0x47:
            //        {
            //            output = "inc " + RegBitmap16((byte)(op - 0x40));
            //            break;
            //        }

            //    // DEC instructions
            //    case 0x48: case 0x49: case 0x4a: case 0x4b: case 0x4c: case 0x4d: case 0x4e: case 0x4f:
            //        {
            //            output = "dec " + RegBitmap16((byte)(op - 0x48));
            //            break;
            //        }
            //    #endregion

            //    #region "0x50-0x5f"
            //    // PUSH instructions
            //    case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x56: case 0x57:
            //        {
            //            output = "push " + RegBitmap16((byte)(op - 0x50));
            //            break;
            //        }

            //    // POP instructions
            //    case 0x58: case 0x59: case 0x5a: case 0x5b: case 0x5c: case 0x5d: case 0x5e: case 0x5f:
            //        {
            //            output = "pop " + RegBitmap16((byte)(op - 0x58));
            //            break;
            //        }
            //    #endregion

            //    #region "0x70-0x7f"
            //        // TODO: function to read immediate
            //    case 0x70:
            //        {
            //            output = "jo ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x71:
            //        {
            //            output = "jno ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x72:
            //        {
            //            output = "jb ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x73:
            //        {
            //            output = "jnb ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x74:
            //        {
            //            output = "jz ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x75:
            //        {
            //            output = "jnz ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x76:
            //        {
            //            output = "jbe ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x77:
            //        {
            //            output = "ja ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x78:
            //        {
            //            output = "js ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x79:
            //        {
            //            output = "jns ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x7a:
            //        {
            //            output = "jpe";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x7b:
            //        {
            //            output = "jpo ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x7c:
            //        {
            //            output = "jl ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x7d:
            //        {
            //            output = "jge ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x7e:
            //        {
            //            output = "jle ";
            //            bytes_read = 2;
            //            break;
            //        }
            //    case 0x7f:
            //        {
            //            output = "jge ";
            //            bytes_read = 2;
            //            break;
            //        }

            //    #endregion

            //    #region "0x90-0x9f"
            //    // NOP
            //    case 0x90:
            //        {
            //            output = "nop";
            //            break;
            //        }
                    
            //    // XCHG instructions
            //    case 0x91: case 0x92: case 0x93: case 0x94: case 0x95: case 0x96: case 0x97: 
            //        {
            //            output = "xchg " + RegBitmap16((byte)(op - 0x90)) + ",ax"; // no offset 0
            //            break;
            //        }

            //    // CBW
            //    case 0x98:
            //        {
            //            output = "cbw";
            //            break;
            //        }

            //    // CWD
            //    case 0x99:
            //        {
            //            output = "cdw";
            //            break;
            //        }
                
            //    // CALL 32 bit segment:offset pointer
            //    case 0x9a:
            //        {
            //            output = "call " + buffer[pc + 4].ToString("X2") + buffer[pc + 3].ToString("X2") + ":" + buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2");
            //            bytes_read = 5;
            //            break;
            //        }

            //    // WAIT
            //    case 0x9b:
            //        {
            //            output = "wait";
            //            break;
            //        }

            //    // PUSHF
            //    case 0x9c:
            //        {
            //            output = "pushf";
            //            break;
            //        }

            //    // POPF
            //    case 0x9d:
            //        {
            //            output = "popf";
            //            break;
            //        }

            //    // SAHF
            //    case 0x9e:
            //        {
            //            output = "sahf";
            //            break;
            //        }

            //    // LAHF
            //    case 0x9f:
            //        {
            //            output = "lahf";
            //            break;
            //        }
            //    #endregion

            //    #region "0xa0-0xaf"
            //    // STOSB
            //    case 0xaa:
            //        {
            //            output = "stosb";
            //            break;
            //        }

            //    // STOSW
            //    case 0xab:
            //        {
            //            output = "stosw";
            //            break;
            //        }

            //    // LODSB
            //    case 0xac:
            //        {
            //            output = "lodsb";
            //            break;
            //        }

            //    // LODSW
            //    case 0xad:
            //        {
            //            output = "lodsw";
            //            break;
            //        }

            //    // SCASB
            //    case 0xae:
            //        {
            //            output = "scasb";
            //            break;
            //        }

            //    // SCASW
            //    case 0xaf:
            //        {
            //            output = "scasw";
            //            break;
            //        }
            //    #endregion

            //    #region "0xb0-0xbf"
            //    // MOV instructions
            //    case 0xb8: case 0xb9: case 0xba: case 0xbb: case 0xbc: case 0xbd: case 0xbe: case 0xbf:
            //        {
            //            output = "mov " + RegBitmap16((byte)(op - 0x58)) + ","; // need to add immediate addr
            //            break;
            //        }
            //    #endregion

            //    #region "0xc0-0xcf"
            //    // RET
            //    case 0xc3:
            //        {
            //            output = "ret";
            //            break;
            //        }

            //    // INTO
            //    case 0xce:
            //        {
            //            output = "into";
            //            break;
            //        }

            //    // IRET
            //    case 0xcf:
            //        {
            //            output = "iret";
            //            break;
            //        }
            //    #endregion

            //    #region "0xd0-0xdf"
            //    // XLAT
            //    case 0xd7:
            //        {
            //            output = "xlat";
            //            break;
            //        }
            //    #endregion

            //    #region "0xf0-0xff"
            //    // LOCK
            //    case 0xf0:
            //        {
            //            output = "lock";
            //            break;
            //        }

            //    // REPNZ
            //    case 0xf2:
            //        {
            //            output = "repnz";
            //            break;
            //        }

            //    // REPZ
            //    case 0xf3:
            //        {
            //            output = "repz";
            //            break;
            //        }

            //    // HLT
            //    case 0xf4:
            //        {
            //            output = "hlt";
            //            break;
            //        }

            //    // CMC
            //    case 0xf5:
            //        {
            //            output = "cmc";
            //            break;
            //        }

            //    // CLC
            //    case 0xf8:
            //        {
            //            output = "clc";
            //            break;
            //        }

            //    // STC
            //    case 0xf9:
            //        {
            //            output = "stc";
            //            break;
            //        }

            //    // CLI
            //    case 0xfa:
            //        {
            //            output = "cli";
            //            break;
            //        }

            //    // STI
            //    case 0xfb:
            //        {
            //            output = "sti";
            //            break;
            //        }

            //    // CLD
            //    case 0xfc:
            //        {
            //            output = "cld";
            //            break;
            //        }

            //    // STD
            //    case 0xfd:
            //        {
            //            output = "std";
            //            break;
            //        }
            //    #endregion

            //    default:
            //        {
            //            output = "op " + op.ToString("X2") + " is undefined";
            //            bytes_read = 1;
            //            break;
            //        }
            //}
            #endregion

            return bytes_read;
        }

        private static string RegBitmap16(byte offset)
        {
            string output = "";
            if (offset == 0)
                output = "ax";
            else if (offset == 1)
                output = "cx";
            else if (offset == 2)
                output = "dx";
            else if (offset == 3)
                output = "bx";
            else if (offset == 4)
                output = "sp";
            else if (offset == 5)
                output = "bp";
            else if (offset == 6)
                output = "si";
            else if (offset == 7)
                output = "di";
            else
                output = "undefined";
            return output;
        }

        private static bool IsOperSizeByte(byte op)
        {
            return ((byte)(op & 0x01) == 0x01);
        }

        private static byte GetMOD(byte data)
        {
            return (byte)(data & 0xc0 >> 6);
        }

        private static byte GetREG(byte data)
        {
            return (byte)(data & 0x38 >> 3);
        }

        private static byte GetRM(byte data)
        {
            return (byte)(data & 0x07);
        }

        private static string GetRMOperand(byte mod, byte rm, int w)
        {
            string result = "";
            switch(mod)
            {
                case 0x00:
                    {
                        result = RMTable1[mod];
                        break;
                    }
                case 0x01:
                    {
                        result = RMTable2[mod];
                        break;
                    }
                case 0x02:
                    {
                        result = RMTable2[mod];
                        break;
                    }
                case 0x03:
                    {
                        result = RegField[rm, w];
                        break;
                    }
                default:
                    {
                        result = " ** UNDEFINED ** ";
                        break;
                    }
            }
            return result;
        }

    }
}
