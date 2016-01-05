using System;
using System.Text;
using KDS.Loader;

namespace KDS.e8086
{
    public class Disassemble8086 : ICodeDisassembler
    {
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
            //UInt16 opCode = i8080Table.opCodes[buffer[pc]].op;
            //string descr = i8080Table.opCodes[buffer[pc]].descr;
            //UInt16 bytes_to_read = i8080Table.opCodes[buffer[pc]].size;
            //int first_space = descr.IndexOf(" ");
            //string first_word;
            //UInt32 pc_output = pc + startingAddress;

            //if (first_space > 0)
            //{
            //    first_word = descr.Substring(0, first_space);
            //    descr = descr.Substring(first_space + 1);
            //}
            //else
            //{
            //    first_word = descr;
            //    descr = string.Empty;
            //}

            //output = "$" + pc_output.ToString("X4") + "\t" + first_word;

            //if (descr.Contains("D16"))
            //{
            //    output += "\t" + descr.Replace("D16", "#$" + buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2"));
            //}
            //else if (descr.Contains("D8"))
            //{
            //    output += "\t" + descr.Replace("D8", "$" + buffer[pc + 1].ToString("X2"));
            //}
            //else if (descr.Contains("adr"))
            //{
            //    output += "\t" + descr.Replace("adr", "$" + buffer[pc + 2].ToString("X2") + buffer[pc + 1].ToString("X2"));
            //}
            //else
            //{
            //    if (descr.Length > 0)
            //        output += "\t" + descr;
            //}

            //if (bytes_to_read == 0) bytes_to_read = 1;
            //return bytes_to_read;
            return 0;
        }
    }
}
