using System.IO;
using System.Text;
using System;
using KDS.e8086;

namespace ConsoleCPUTest
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var cpu = new CPU();
            var program = LoadFile(@"C:\Users\adam\source\repos\e8086\Resources\codegolf.bin");
            cpu.Boot(0, 0, program);
            cpu.EU.Registers.SP = 0x0100;

            do
            {
                cpu.NextInstruction();

            } while (cpu.Bus.IP != 0x0006 && cpu.Bus.IP != 0x0143);
            WriteScreen(cpu.Bus);
            Console.ReadKey();
        }

        private static void WriteScreen(IBus bus)
        {
            ushort baseAddress = 0x8000;
            StringBuilder scn = new StringBuilder(2048);

            for(ushort row = 0; row < 25; row++)
            {
                for(ushort col = 0; col < 80; col++)
                {
                    char ch = (char) bus.GetData(0, baseAddress++);
                    if (ch == '\0')
                    {
                        scn.Append(" ");
                    }
                    else
                    {
                        scn.Append(ch);
                    }
                }
                scn.AppendLine();
            }
            Console.Clear();
            Console.Write(scn.ToString());
        }

        private static byte[] LoadFile(string fileName)
        {
            byte[] buffer;
            FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read);

            try
            {
                int length = (int)fs.Length;
                buffer = new byte[length];
                int bytes_read;
                int total_bytes = 0;

                do
                {
                    bytes_read = fs.Read(buffer, total_bytes, length - total_bytes);
                    total_bytes += bytes_read;
                } while (bytes_read > 0);
            }
            finally
            {
                fs.Close();
            }
            return buffer;
        }

        
    }
}
