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
            UInt32 bytes_read = 1; // 1 for the program counter

            // these are unimplemented instructions from the op code table
            if (i8086Table.opCodes[op].num_operands == -1)
                throw new NotImplementedException("The instruction " + op.ToString("X2") + " is not implemented");

            // with no operands just return the string directly from the table
            if (i8086Table.opCodes[op].num_operands == 0)
            {
                output = i8086Table.opCodes[op].dasm_format;
            }
            else if (i8086Table.opCodes[op].num_operands == 1)
            {
                throw new NotImplementedException("Instruction " + op.ToString("X2") + " has 1 operand and is not implemented");
            }

            // special processing for instructions with 2 operands
            else // 2 operands
            {
                string operand1 = "";
                string operand2 = "";
                int w = GetW(op);
                byte reg = GetREG(buffer[pc + 1]);
                OpCodeTable oc = i8086Table.opCodes[op];
                bool imm_in_addr = false;

                // operand1
                if (oc.arg1_mode == "R/M")
                {
                    bytes_read += GetRMOperand(buffer, pc, out operand1);
                }
                else if (oc.arg1_mode == "REG")
                {
                    operand1 = RegField[reg, w];
                    bytes_read += 1;  // 1 for the address byte
                }
                else if (oc.arg1_mode == "I")
                {
                    if (w == 0)
                    {
                        operand1 = "[" + buffer[pc + 2].ToString("X2");
                        bytes_read += 1;
                    }
                    else
                    {
                        operand1 = "[" + buffer[pc + 3].ToString("X2") + buffer[pc + 2].ToString("X2");
                        bytes_read += 2;
                    }
                }
                else
                {
                    operand1 = oc.arg1_mode;
                    imm_in_addr = true;
                }

                // operand2
                if (oc.arg2_mode == "R/M")
                {
                    bytes_read += GetRMOperand(buffer, pc, out operand2);
                }
                else if (oc.arg2_mode == "REG")
                {
                    operand2 = RegField[reg, w];
                    bytes_read += 1;  // 1 for the address byte
                }
                else if (oc.arg2_mode == "I")
                {
                    if (w == 0)
                    {
                        if (imm_in_addr)
                        {
                            operand2 = buffer[pc + 1].ToString("X2");
                        }
                        else
                        {
                            operand2 = buffer[pc + 2].ToString("X2");
                            bytes_read += 1;
                        }
                    }
                    else
                    {
                        if (imm_in_addr)
                        {
                            operand2 = buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2");
                            bytes_read += 1;
                        }
                        else
                        {
                            operand2 = buffer[pc + 3].ToString("X2") + buffer[pc + 2].ToString("X2");
                            bytes_read += 2;
                        }
                    }

                }
                else
                {
                    operand2 = oc.arg2_mode;
                }

                output = string.Format(oc.dasm_format, operand1, operand2);
            }

            output = output.ToLower();
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

        private static byte GetMOD(byte data)
        {
            return (byte)((data & 0xc0) >> 6);
        }

        private static byte GetREG(byte data)
        {
            return (byte)((data & 0x38) >> 3);
        }

        private static byte GetRM(byte data)
        {
            return (byte)(data & 0x07);
        }
        
        private static int GetW(byte op)
        {
            return ((byte)(op & 0x01) == 0x01) ? 1 : 0;
        }

        private static UInt32 GetRMOperand(byte[] buffer, UInt32 pc, out string operand)
        {
            UInt32 bytes_read = 1; // 1 for the address byte
            byte op = buffer[pc];
            byte addr_byte = buffer[pc + 1];
            pc++;  // increment pc for reading the addr byte

            int w = ((byte)(op & 0x01) == 0x01) ? 1 : 0;
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


    }
}
