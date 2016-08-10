using System;
using System.Text;

namespace KDS.e8086
{
    public class Disassemble8086 
    {
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
            "bx+si","bx+di","bp+si","bp+di","si","di","direct","bx"
        };

        public static string[] RMTable2 = new string[]
        {
            "bx+si","bx+di","bp+si","bp+di","si","di","bp","bx"
        };

        public static string[] SegRegTable = new string[]
        {
            "es","cs","ss","ds","fs"
        };

        // op codes 0x80, 0x81, 0x82, 0x83 (name == GRP1)  
        public static string[] Group1Table = new string[]
        {
            "add", "or", "adc", "sbb", "and", "sub", "xor", "cmp"
        };

        // op codes 0xd0, 0xd1, 0xd2, 0xd3 (name == GRP2)
        public static string[] Group2Table = new string[]
        {
            "rol", "ror", "rcl", "rcr", "shl", "shr","", "sar"
        };

        // op codes 0xf6, 0f7 (name == GRP3)
        public static string[] Group3Table = new string[]
        {
            "test","", "not", "neg", "mul", "imul", "div", "idiv"
        };

        // op code 0xfe (name == GRP4)
        public static string[] Group4Table = new string[]
        {
            "inc","dec", "", "", "", "", "", ""
        };

        // op code 0xff (name == GRP5)
        public static string[] Group5Table = new string[]
        {
            "inc","dec","call","call","jmp","jmp","push",""
        };

        public static string Disassemble(byte[] data, ushort startingAddress)
        {
            StringBuilder output = new StringBuilder();

            uint pc = 0;
            string line;

            while (pc < data.Length)
            {
                output.Append(pc.ToString("X4") + "\t\t");
                pc += DisassembleNext(data, pc, startingAddress, out line);
                output.AppendLine(line);
            }

            return output.ToString();
        }

        public static uint DisassembleNext(byte[] buffer, uint pc, ushort startingAddress, out string output)
        {
            // formatting note:  immediate data does not get brackets
            // data from memory or calculations get brackets

            // something went terribly wrong
            if (buffer.Length <= pc)
                throw new IndexOutOfRangeException("Index pc exceeds the bounds of the buffer");

            uint bytes_read = 1;
            byte op = buffer[pc];
            OpCodeDasmRecord op_table = OpCodeDasmTable.opCodes[op];

            output = "";

            // if there is no entry for this code in the op_table it is unused
            if (op_table.op_name == "")
            {
                output = "UNDEFINED OP CODE " + op.ToString("X2");
            }

            // if there is no info for the 2nd byte then it is a one byte op code with a static string
            else if (op_table.addr_byte == "")
            {
                output = op_table.op_name;
            }

            // Otherwise use the addr_byte format to determine next steps
            // Possible values: A-LO, D-8, D-LO, IP-INC-8, IP-INC-LO, IP-LO, MRR

            // A-LO: 2nd & 3rd bytes point to data at a memory location
            else if (op_table.addr_byte == "A-LO")
            {
                DataRegister16 reg21 = new DataRegister16(buffer[pc + 2], buffer[pc + 1]);
                bytes_read += 2;
                if (op_table.op_fmt_1.StartsWith("M-"))
                {
                    output = string.Format("{0} [{1}],{2}", op_table.op_name,
                                             reg21.ToString(),
                                             op_table.op_fmt_2);
                }
                else
                {
                    output = string.Format("{0} {1},[{2}]", op_table.op_name,
                                             op_table.op_fmt_1,
                                             reg21.ToString());
                }
            }

            // The 2nd byte is 8 bit immediate data
            else if (op_table.addr_byte == "D-8")
            {
                bytes_read++; // these are two byte instructions

                if (op_table.op_fmt_1 == "I-8")
                {
                    if (op_table.op_fmt_2 == "")
                    {
                        output = string.Format("{0} {1}", op_table.op_name, buffer[pc + 1].ToString("X2"));
                    }
                    else
                    {
                        output = string.Format("{0} {1},{2}", op_table.op_name, buffer[pc + 1].ToString("X2"), op_table.op_fmt_2);
                    }
                }
                else
                {
                    output = string.Format("{0} {1},{2}", op_table.op_name, op_table.op_fmt_1, buffer[pc + 1].ToString("X2"));
                }
            }

            // D-LO: 2nd & 3rd bytes are 16 bit immediate data
            else if (op_table.addr_byte == "D-LO")
            {
                DataRegister16 reg21 = new DataRegister16(buffer[pc + 2], buffer[pc + 1]);

                bytes_read += 2; // these are three byte instructions

                if (op_table.op_fmt_1 == "I-16")
                {
                    output = string.Format("{0} {1}", op_table.op_name,
                                            reg21.ToString());

                }
                else if (op_table.op_fmt_1 == "FAR")
                {
                    DataRegister16 reg43 = new DataRegister16(buffer[pc + 4], buffer[pc + 3]);

                    output = string.Format("{0} {1}:{2}", op_table.op_name,
                                            reg43.ToString(),
                                            reg21.ToString());
                    bytes_read += 2; // two additional bytes
                }
                else
                {
                    output = string.Format("{0} {1},{2}", op_table.op_name, op_table.op_fmt_1,
                                            reg21.ToString());
                }
            }

            // IP-INC-8: Add the 8 bit number to the value of the program counter for the next instruction
            else if (op_table.addr_byte == "IP-INC-8")
            {
                uint immediate = (pc + 2) + buffer[pc + 1];
                bytes_read++; // two byte instruction

                output = string.Format("{0} {1} {2}", op_table.op_name, op_table.op_fmt_1, immediate.ToString("X4"));
            }

            // IP-INC-LO: 2nd & 3rd bytes are 16 bites of immediate data to be added to the program counter of the
            // next instruction
            else if (op_table.addr_byte == "IP-INC-LO")
            {
                DataRegister16 reg21 = new DataRegister16(buffer[pc + 2], buffer[pc + 1]);

                uint immediate = (pc + 3) + reg21;
                bytes_read += 2; // three byte instruction

                output = string.Format("{0} {1}", op_table.op_name, immediate.ToString("X4"));
            }

            // IP-LO: next 4 bytes are IP-LO, IP-HI, CS-LO, CS-HI
            else if (op_table.addr_byte == "IP-LO")
            {
                bytes_read += 4; // five byte instruction
                DataRegister16 reg21 = new DataRegister16(buffer[pc + 2], buffer[pc + 1]);
                DataRegister16 reg43 = new DataRegister16(buffer[pc + 4], buffer[pc + 3]);

                output = string.Format("{0} {1}:{2}", op_table.op_name, reg43.ToString(), reg21.ToString());

            }

            // MRR: the biggie
            else if (op_table.addr_byte == "MRR")
            {
                byte addr_mode = buffer[pc + 1];
                bytes_read++;

                // possible formatting values:
                // M-8, M-16, I-8, I-16, R-8, R-16, RM-8, RM-16, SEG
                string oper1 = "";
                string oper2 = "";

                string oper1_fmt = GetFirstOper(buffer, pc);
                if (oper1_fmt == "RM-8" || oper1_fmt == "M-8")
                {
                    bytes_read += DisassembleRM(buffer, pc, 1, GetMOD(addr_mode), GetRM(addr_mode), 0, out oper1);
                }
                else if (oper1_fmt == "RM-16" || oper1_fmt == "M-16")
                {
                    bytes_read += DisassembleRM(buffer, pc, 1, GetMOD(addr_mode), GetRM(addr_mode), 1, out oper1);
                }
                else if (oper1_fmt == "R-8")
                {
                    oper1 = RegField[GetREG(buffer, pc), 0];
                }
                else if (oper1_fmt == "R-16")
                {
                    oper1 = RegField[GetREG(buffer, pc), 1];
                }
                else if (oper1_fmt == "SEG")
                {
                    oper1 = SegRegTable[GetREG(buffer, pc)];
                }
                else if (oper1_fmt == "I-8")
                {
                    oper1 = buffer[pc + 2].ToString("X2");
                    bytes_read++;
                }
                else if (oper1_fmt == "I-16")
                {
                    oper1 = buffer[pc + 3].ToString("X2") + buffer[pc + 2].ToString("X2");
                    bytes_read += 2;
                }
                else if (oper1_fmt != "")
                {
                    oper1 = oper1_fmt;
                }

                uint immed_offset = bytes_read - 1;

                if (!SkipSecondOper(buffer, pc))
                {
                    if (op_table.op_fmt_2 == "RM-8" || op_table.op_fmt_2 == "M-8")
                    {
                        bytes_read += DisassembleRM(buffer, pc, immed_offset, GetMOD(addr_mode), GetRM(addr_mode), 0, out oper2);
                    }
                    else if (op_table.op_fmt_2 == "RM-16" || op_table.op_fmt_2 == "M-16")
                    {
                        bytes_read += DisassembleRM(buffer, pc, immed_offset, GetMOD(addr_mode), GetRM(addr_mode), 1, out oper2);
                    }
                    else if (op_table.op_fmt_2 == "R-8")
                    {
                        oper2 = RegField[GetREG(buffer, pc), 0];
                    }
                    else if (op_table.op_fmt_2 == "R-16")
                    {
                        oper2 = RegField[GetREG(buffer, pc), 1];
                    }
                    else if (op_table.op_fmt_2 == "SEG")
                    {
                        oper2 = SegRegTable[GetREG(buffer, pc)];
                    }
                    else if (op_table.op_fmt_2 == "I-8")
                    {
                        oper2 = buffer[pc + immed_offset + 1].ToString("X2");
                        bytes_read++;
                    }
                    else if (op_table.op_fmt_2 == "I-16")
                    {
                        oper2 = buffer[pc + immed_offset + 2].ToString("X2") + buffer[pc + immed_offset + 1].ToString("X2");
                        bytes_read += 2;
                    }
                    else if (op_table.op_fmt_2 != "")
                    {
                        oper2 = op_table.op_fmt_2;
                    }
                }

                if (oper2 == "")
                {
                    output = string.Format("{0} {1}", GetOpName(op, GetREG(buffer,pc)), oper1);
                }
                else
                {
                    output = string.Format("{0} {1},{2}", GetOpName(op, GetREG(buffer, pc)), oper1, oper2);
                }
            }

            output = output.ToLower();
            return bytes_read;
        }

        private static uint DisassembleRM(byte[] buffer, uint pc, uint offset, byte mod, byte rm, byte word_oper, out string output)
        {
            uint bytes_read = 0;
            output = "";

            if (mod == 0x00) // R/M Table 1
            {
                if (rm == 0x06) // direct address, special case
                {
                    output = "[" + buffer[pc + offset + 2].ToString("X2") + buffer[pc + offset + 1].ToString("X2") + "]";
                    bytes_read += 2;
                }
                else
                {
                    output = "[" + RMTable1[rm] + "]";
                }
            }
            else if (mod == 0x01) // R/M Table 2 with 8 bit displacement
            {
                output = "[" + RMTable2[rm] + "+" + buffer[pc + offset+ 1].ToString("X2") + "]";
                bytes_read += 1;
            }
            else if (mod == 0x02) // R/M Table 2 with 16 bit displacement
            {
                output = "[" + RMTable2[rm] + "+" + buffer[pc + offset + 2].ToString("X2") + buffer[pc + offset + 1].ToString("X2") + "]";
                bytes_read += 2;
            }
            else if (mod == 0x03) // Use REG table with R/M value
            {
                output = RegField[rm, word_oper];
            }
            else
                throw new InvalidOperationException("The value of mod " + mod.ToString("X2") + " is invalid");


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

        private static string GetOpName(byte op, byte reg)
        {
            string output = OpCodeDasmTable.opCodes[op].op_name;
            if (output == "GRP1")
            {
                output = Group1Table[reg];
            }
            else if( output == "GRP2")
            {
                output = Group2Table[reg];
            }
            else if (output == "GRP3")
            {
                output = Group3Table[reg];
            }
            else if (output == "GRP4")
            {
                output = Group4Table[reg];
            }
            else if (output == "GRP5")
            {
                output = Group5Table[reg];
            }

            return output;
        }

        private static byte GetMOD(byte data)
        {
            return (byte)((data & 0xc0) >> 6);
        }

        private static byte GetREG(byte[] buffer, uint pc)
        {
            // op codes that begin with 1011 are formatted 1011wreg
            byte reg;
            byte op = buffer[pc];
            byte data = buffer[pc + 1];

            if ((byte)(op & 0xf0) == 0xb0)
            {
                reg = (byte)(op & 0x03);
            }
            else
            {
                reg = (byte)((data & 0x38) >> 3);
            }
            return reg;
        }

        private static byte GetRM(byte data)
        {
            return (byte)(data & 0x07);
        }
        
        private static int GetW(byte op)
        {
            // op codes that begin with 1011 (0xb) are formatted: 1011wreg
            int w;
            if ((byte)(op & 0xf0) == 0xb0)
            {
                w = ((byte)(op & 0x08) == 0x08) ? 1 : 0;
            }
            // the les instruction uses 16 bit lookup table
            else if( op == 0xc4)
            {
                w = 1;
            }
            else
            {
                w = ((byte)(op & 0x01) == 0x01) ? 1 : 0;
            }
            return w;
        }

        private static string GetAddrMode(byte addr_byte, bool first)
        {
            int d = ((byte)(addr_byte & 0x02) == 0x02) ? 1 : 0;
            string mode;
            if(( first && d == 0 ) || (!first && d == 1))
            {
                mode = "R/M";
            }
            else 
            {
                mode = "REG";
            }
            return mode;
        }

        private static uint GetRMOperand(byte[] buffer, uint pc, bool with_segment, out string operand)
        {
            uint bytes_read = 0; // report extra after address byte
            byte op = buffer[pc];
            byte addr_byte = buffer[pc + 1];
            pc++;  // increment pc for reading the addr byte

            // If this is a two operand instruction and the other operand uses a segment register then
            // the R/M reference is always by word and not byte.  
            int w;
            if (with_segment)
                w = 1;
            else
                w = GetW(op); 

            byte mod = GetMOD(addr_byte);
            byte rm = GetRM(addr_byte);
            operand = "";

            if (mod == 0x00) // R/M Table 1
            {
                if (rm == 0x06) // then direct address
                {
                    if (w == 1) // 16 bit immediate
                    {
                        operand = "[" + buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2") + "]";
                        bytes_read += 2;
                    }
                    else // 8 bit immediate
                    {
                        operand = "[" + buffer[pc + 1].ToString("X2") + "]";
                        bytes_read += 1;
                    }
                }
                else
                {
                    operand = "[" + RMTable1[rm] + "]";
                }
            }
            else if (mod == 0x01) // R/M Table 2 with 8 bit displacement
            {
                operand = "[" + RMTable2[rm] + "+" + buffer[pc + 1].ToString("X2") + "]";
                bytes_read += 1;
            }
            else if (mod == 0x02) // R/M Table 2 with 16 bit displacement
            {
                operand = "[" + RMTable2[rm] + "+" + buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2") + "]";
                bytes_read += 2;
            }
            else if (mod == 0x03) // Use REG table with R/M value
            {
                operand = RegField[rm, w];
            }
            else
            {
                throw new InvalidOperationException("mod value of " + mod.ToString("X2") + " is invalid");
            }

            return bytes_read;
        }

        private static string GetFirstOper(byte[] buffer, uint pc)
        {
            byte op = buffer[pc];
            string output = OpCodeDasmTable.opCodes[op].op_fmt_1;

            if( op == 0xff )
            {
                byte reg = GetREG(buffer, pc);
                if (reg == 0x02 || reg == 0x04)
                {
                    output = "RM-16";
                }
                else
                {
                    output = "M-16";
                }
            }
            return output;
        }

        private static bool SkipSecondOper(byte[] buffer, uint pc)
        {
            bool skip = false;
            byte op = buffer[pc];

            // special case, the 2nd oper is used only if REG == 0x00
            if( op == 0xf6 || op == 0xf7 )
            {
                if( GetREG(buffer, pc) != 0x00 )
                {
                    skip = true;
                }
            }
            return skip;
        }

    }
}
