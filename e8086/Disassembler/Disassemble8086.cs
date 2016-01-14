using System;
using System.Text;
using System.Diagnostics;
using KDS.Loader;

namespace KDS.e8086
{
    public class Disassemble8086 : ICodeDisassembler
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
            "es","cs","ss","ds"
        };

        public static string[] Group1Table = new string[]
        {
            "add", "or", "adc", "sbb", "and", "sub", "xor", "cmp"
        };

        public static string[] Group2Table = new string[]
        {
            "rol", "ror", "rcl", "rcr", "shl", "shr","", "sar"
        };

        public static string[] Group3Table = new string[]
        {
            "test","", "not", "neg", "mul", "imul", "div", "idiv"
        };

        public static string[] Group4Table = new string[]
        {
            "inc","dec", "", "", "", "", "", ""
        };

        public static string[] Group5Table = new string[]
        {
            "inc","dec","call","call","jmp","jmp","push",""
        };

        public string Disassemble(ICodeLoader loader)
        {
            return Disassemble(loader, 0x00);
        }

        public string Disassemble(ICodeLoader loader, UInt16 startingAddress)
        {
            byte[] buffer = loader.LoadData();
            StringBuilder output = new StringBuilder();

            UInt32 pc = 0;
            string line;

            while (pc < buffer.Length)
            {
                output.Append(pc.ToString("X4") + "\t\t");
                pc += DisassembleNext(buffer, pc, startingAddress, out line);
                Debug.WriteLine(line);
                output.AppendLine(line);
            }

            return output.ToString();
        }

        public static UInt32 DisassembleNext(byte[] buffer, UInt32 pc, UInt16 startingAddress, out string output)
        {
            // formatting note:  immediate data does not get brackets
            // data from memory or calculations get brackets

            // something went terribly wrong
            if (buffer.Length <= pc)
                throw new IndexOutOfRangeException("Index pc exceeds the bounds of the buffer");

            UInt32 bytes_read = 1;
            byte op = buffer[pc];
            OpCodeTable op_table = i8086Table.opCodes[op];
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
                bytes_read += 2;

                if (op_table.op_fmt_1.StartsWith("M-"))
                {
                    output = string.Format("{0} [{1}],{2}", op_table.op_name,
                                             GetValue16(buffer[pc + 2], buffer[pc + 1]).ToString("X4"),
                                             op_table.op_fmt_2);
                }
                else
                {
                    output = string.Format("{0} {1},[{2}]", op_table.op_name,
                                             op_table.op_fmt_1,
                                             GetValue16(buffer[pc + 2], buffer[pc + 1]).ToString("X4"));
                }
            }

            // The 2nd byte is 8 bit immediate data
            else if (op_table.addr_byte == "D-8")
            {
                bytes_read++; // these are two byte instructions

                if (op_table.op_fmt_1 == "I-8")
                {
                    output = string.Format("{0} {1},{2}", op_table.op_name, buffer[pc + 1].ToString("X2"), op_table.op_fmt_2);
                }
                else
                {
                    output = string.Format("{0} {1},{2}", op_table.op_name, op_table.op_fmt_1, buffer[pc + 1].ToString("X2"));
                }
            }

            // D-LO: 2nd & 3rd bytes are 16 bit immediate data
            else if (op_table.addr_byte == "D-LO")
            {
                bytes_read += 2; // these are three byte instructions

                if (op_table.op_fmt_1 == "I-16")
                {
                    output = string.Format("{0} {1}", op_table.op_name,
                                            GetValue16(buffer[pc + 2], buffer[pc + 1]).ToString("X4"));

                }
                else if (op_table.op_fmt_1 == "FAR")
                {
                    output = string.Format("{0} {1}:{2}", op_table.op_name,
                                            GetValue16(buffer[pc + 4], buffer[pc + 3]).ToString("X4"),
                                            GetValue16(buffer[pc + 2], buffer[pc + 1]).ToString("X4"));
                    bytes_read += 2; // two additional bytes
                }
                else
                {
                    output = string.Format("{0} {1},{2}", op_table.op_name, op_table.op_fmt_1,
                                            GetValue16(buffer[pc + 2], buffer[pc + 1]).ToString("X4"));
                }
            }

            // IP-INC-8: Add the 8 bit number to the value of the program counter for the next instruction
            else if( op_table.addr_byte == "IP-INC-8")
            {
                UInt32 immediate = pc + 2 + buffer[pc+1];
                bytes_read++; // two byte instruction

                output = string.Format("{0} {1} {2}", op_table.op_name, op_table.op_fmt_1, immediate.ToString("X4"));
            }

            // IP-INC-LO: 2nd & 3rd bytes are 16 bites of immediate data to be added to the program counter of the
            // next instruction
            else if (op_table.addr_byte == "IP-INC-LO")
            {
                UInt32 immediate = pc + 3 + GetValue16(buffer[pc + 2], buffer[pc + 1]);
                bytes_read += 2; // three byte instruction

                output = string.Format("{0} {2}", op_table.op_name, immediate.ToString("X4"));
            }

            // IP-LO: next 4 bytes are IP-LO, IP-HI, CS-LO, CS-HI
            else if( op_table.addr_byte == "IP-LO" )
            {
                bytes_read += 4; // five byte instruction
                output = string.Format("{0} {1}:{2}", op_table.op_name,
                                                        GetValue16(buffer[pc + 4], buffer[pc + 3]).ToString("X4"),
                                                        GetValue16(buffer[pc + 2], buffer[pc + 1]).ToString("X4"));

            }

            // MRR: the biggie
            else if( op_table.addr_byte == "MRR")
            {
                output = op_table.op_name;
            }

            output = output.ToLower();
            return bytes_read;
        }


        //public static UInt32 DisassembleNext_Old(byte[] buffer, UInt32 pc, UInt16 startingAddress, out string output)
        //{
        //    if (buffer.Length <= pc)
        //        throw new IndexOutOfRangeException("Index pc exceeds the bounds of the buffer");

        //    byte op = buffer[pc];
        //    output = "";
        //    UInt32 bytes_read = 1; // 1 for the program counter

        //    // these are unimplemented instructions from the op code table
        //    if (i8086Table.opCodes[op].num_operands == -1)
        //        throw new NotImplementedException("The instruction " + op.ToString("X2") + " is not implemented");

        //    // with no operands just return the string directly from the table
        //    if (i8086Table.opCodes[op].num_operands == 0)
        //    {
        //        output = i8086Table.opCodes[op].addr_byte;
        //    }

        //    // instructions with 1 operand
        //    else if (i8086Table.opCodes[op].num_operands == 1)
        //    {
        //        OpCodeTable oc = i8086Table.opCodes[op];
        //        string op_string = "";
        //        byte reg = GetREG(buffer,pc);
        //        int w = GetW(op);
        //        if (op == 0xf6 || op == 0xf7)
        //        {
        //            op_string = Group3Table[reg];
        //        }
        //        else if (op == 0xfe)
        //        {
        //            op_string = Group4Table[reg];
        //        }
        //        else if (op == 0xff)
        //        {
        //            op_string = Group5Table[reg];
        //        }

        //        if (oc.next_bytes == "R/M")
        //        {
        //            string operand;
        //            bytes_read += 1; // one for the address byte
        //            bytes_read += GetRMOperand(buffer, pc, false, out operand);
        //            if( op_string == "")
        //                output = string.Format(i8086Table.opCodes[op].addr_byte, operand);
        //            else
        //                output = string.Format(i8086Table.opCodes[op].addr_byte, op_string, operand);
        //        }
        //        else if (oc.next_bytes == "J")
        //        {
        //            // jump commands contain an offset added to the next instruction location

        //            if (w == 0)
        //            {
        //                byte result = (byte)(buffer[pc + 1] + (pc + 2));
        //                output = string.Format(i8086Table.opCodes[op].addr_byte, result.ToString("X2"));
        //                bytes_read += 1;
        //            }
        //            else
        //            {
        //                UInt32 result = GetValue16(buffer[pc + 2], buffer[pc + 1]) + (pc + 3);
        //                output = string.Format(i8086Table.opCodes[op].addr_byte, result.ToString("X4"));
        //                bytes_read += 2;
        //            }
        //        }
        //        else if (oc.next_bytes == "A")
        //        {
        //            // direct address
        //            string addr = buffer[pc + 4].ToString("X2") + buffer[pc + 3].ToString("X2") + ":" +
        //                            buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2");
        //            output = string.Format(i8086Table.opCodes[op].addr_byte, addr);
        //            bytes_read += 4;
        //        }
        //        else if (oc.next_bytes == "I")
        //        {
        //            // one operand instructions with immediate begin with the 2nd byte
        //            if( oc.op_name == "b")
        //            {
        //                output = string.Format(i8086Table.opCodes[op].addr_byte, buffer[pc + 1].ToString("X2"));
        //                bytes_read += 1;
        //            }
        //            else
        //            {
        //                output = string.Format(i8086Table.opCodes[op].addr_byte, buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2"));
        //                bytes_read += 2;
        //            }

        //        }
        //        else
        //            throw new NotImplementedException("Instruction " + op.ToString("X2") + " has 1 operand and is not implemented");
        //    }

        //    // special processing for instructions with 2 operands
        //    else // 2 operands
        //    {
        //        string op_string = "";
        //        string operand1 = "";
        //        string operand2 = "";
        //        int w = GetW(op);
        //        byte reg = GetREG(buffer,pc);
        //        OpCodeTable oc = i8086Table.opCodes[op];
        //        bool imm_in_addr = false;
        //        bytes_read += 1; // one for the address byte

        //        // Group op codes
        //        if (op == 0x80 || op == 0x81 || op == 0x82 || op == 0x83)
        //        {
        //            op_string = Group1Table[reg];
        //        }
        //        // Group op codes
        //        else if (op == 0xd0 || op == 0xd1 || op == 0xd2 || op == 0xd3)
        //        {
        //            op_string = Group2Table[reg];
        //        } 

        //        // operand1
        //        if (oc.next_bytes == "R/M")
        //        {
        //            bytes_read += GetRMOperand(buffer, pc, (oc.op_fmt_1 == "S"), out operand1);
        //        }
        //        else if (oc.next_bytes == "REG")
        //        {
        //            operand1 = RegField[reg, w];
        //        }
        //        else if (oc.next_bytes == "I")
        //        {
        //            if (oc.op_name == "b")
        //            {
        //                operand1 = buffer[pc + 1].ToString("X2");
        //            }
        //            else
        //            {
        //                operand1 = buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2");
        //                bytes_read += 1;
        //            }
        //        }
        //        else if( oc.next_bytes == "S")
        //        {
        //            operand1 = SegRegTable[reg];
        //        }
        //        else if( oc.next_bytes == "O")
        //        {
        //            operand1 = "[" + buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2") + "]";
        //            bytes_read += 1;
        //        }
        //        else
        //        {
        //            operand1 = oc.next_bytes;
        //            imm_in_addr = true;
        //        }

        //        // for operand2 calculate an offset for immediate bytes
        //        UInt32 imm_offset = 0;
        //        if( bytes_read > 2 )
        //        {
        //            imm_offset = bytes_read - 2;
        //        }

        //        // operand2
        //        if (oc.op_fmt_1 == "R/M")
        //        {
        //            bytes_read += GetRMOperand(buffer, pc, (oc.next_bytes == "S"), out operand2);
        //        }
        //        else if (oc.op_fmt_1 == "REG")
        //        {
        //            operand2 = RegField[reg, w];
        //        }
        //        else if (oc.op_fmt_1 == "I")
        //        {
        //            if (w == 0)
        //            {
        //                if (imm_in_addr)
        //                {
        //                    operand2 = buffer[pc + 1].ToString("X2");
        //                }
        //                else
        //                {
        //                    operand2 = buffer[pc + 2 + imm_offset].ToString("X2");
        //                    bytes_read += 1;
        //                }
        //            }
        //            else
        //            {
        //                if (imm_in_addr)
        //                {
        //                    operand2 = buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2");
        //                    bytes_read += 1;
        //                }
        //                else
        //                {
        //                    operand2 = buffer[pc + 3 + imm_offset].ToString("X2") + buffer[pc + 2 + imm_offset].ToString("X2");
        //                    bytes_read += 2;
        //                }
        //            }

        //        }
        //        else if (oc.op_fmt_1 == "S")
        //        {
        //            operand2 = SegRegTable[reg];
        //        }
        //        else if (oc.op_fmt_1 == "O")
        //        {
        //            operand2 = "[" + buffer[pc + 2 + imm_offset].ToString("X2") + buffer[pc + 1 + imm_offset].ToString("X2") + "]";
        //            bytes_read += 1;
        //        }
        //        else
        //        {
        //            operand2 = oc.op_fmt_1;
        //        }

        //        if (op_string != "")
        //        {
        //            output = string.Format(oc.addr_byte, op_string, operand1, operand2);
        //        }
        //        else
        //        {
        //            output = string.Format(oc.addr_byte, operand1, operand2);
        //        }
        //    }

        //    output = output.ToLower();
        //    return bytes_read;
        //}

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

        private static byte GetMOD(byte data)
        {
            return (byte)((data & 0xc0) >> 6);
        }

        private static byte GetREG(byte[] buffer, UInt32 pc)
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

        private static UInt32 GetRMOperand(byte[] buffer, UInt32 pc, bool with_segment, out string operand)
        {
            UInt32 bytes_read = 0; // report extra after address byte
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

        public static UInt16 GetValue16(byte hi, byte lo)
        {
            return (UInt16)(((UInt32)hi << 8 | (UInt32)lo) & 0xffff);
        }
    }
}
