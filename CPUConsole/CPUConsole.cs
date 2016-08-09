using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KDS.Loader;
using KDS.e8086;
using System.IO;
using System.Diagnostics;

namespace CPUConsole
{
    class CPUConsole
    {
        static i8086CPU cpu = new i8086CPU();
        static bool exit = false;
        static long count = 0;
        static Stopwatch sw = new Stopwatch();

        /*
        To make a full 8086 computer the following is needed:
           - XT BIOS
           - Intel 8253 timer 
           - Intel 8259 interrupt controller 
           - video driver

            optional
            - mouse
            - Intel 8237 DMA controller (for soundblaster emulation)
            - network
        */

        static void Main(string[] args)
        {
            //DisassembleThings();

            Console.SetWindowSize(100, 50);
            cpu = new i8086CPU();
            // BIOS
            cpu.Bus.LoadBIOS(FileLoader.LoadFile("C:\\Users\\menschas\\Downloads\\fake86-master\\fake86-master\\data\\pcxtbios.bin"));
            
            // laptop
            //cpu.Boot(FileLoader.LoadFile("C:\\Users\\menschas\\Source\\e8086\\Resources\\codegolf.bin"));
            // desktop
            //cpu.Boot(FileLoader.LoadFile("C:\\Users\\adam\\Documents\\My Projects\\e8086\\Resources\\codegolf.bin"));

            //cpu.EU.Registers.SP = 0x100;

            // Addition overflow test from http://www.c-jump.com/CIS77/ASM/Flags/lecture.html
            //cpu.Boot(new byte[] {
            //                            0xc6, 0x06, 0x00, 0x10, 0x27,
            //                            0xc6, 0xc0, 0x1a,
            //                            0x40,
            //                            0x80, 0xc0, 0x4c,
            //                            0x02, 0x06, 0x00, 0x10,
            //                            0x88, 0xc4,
            //                            0x00, 0xe0
            //});
    
            // Subtraction overflow test from http://www.c-jump.com/CIS77/ASM/Flags/lecture.html
            //cpu.Boot(new byte[] {
            //                            0xc6, 0xc0, 0x5f,
            //                            0x48,
            //                            0x80, 0xe8, 0x17,
            //                            0xc6, 0x06, 0x00, 0x10, 0x7a,
            //                            0x2a, 0x06, 0x00, 0x10,
            //                            //0xb4, 0x77,
            //                            0xc6, 0xc4, 0x77,
            //                            0x28, 0xe0
            //});

            do
            {
                Console.Clear();
                DisplayDebug();
                //DisplayVRAM();
                DisplayMenu();
            } while (!exit);

            //StreamWriter sw = new StreamWriter("C:\\Users\\menschas\\Source\\e8086\\Resources\\codegolf_stats.txt");
            //sw.WriteLine(string.Format("Total Instructions Executed: {0}", cpu.EU.Stats.InstructionCount));
            //sw.WriteLine();
            //sw.WriteLine("Count per op code:");
            //sw.Write(cpu.EU.Stats.GetOpReport());
            //sw.Close();

        }

        static void DisassembleThings()
        {
            string dasm = Disassemble8086.Disassemble(FileLoader.LoadFile("C:\\Users\\adam\\Downloads\\XUBR580\\pcxtbios.bin"), 0);

            StreamWriter sw = new StreamWriter("C:\\Users\\adam\\Downloads\\XUBR580\\pcxtbios.txt");
            sw.Write(dasm);
            sw.Close();

        }

        static void DisplayDebug()
        {
            Console.WriteLine(string.Format("AX: {0:X4}\tBX: {1:X4}\tCX: {2:X4}\tDX: {3:X4}",
                cpu.EU.Registers.AX, cpu.EU.Registers.BX, cpu.EU.Registers.CX, cpu.EU.Registers.DX));

            Console.WriteLine(string.Format("SP: {0:X4}\tBP: {1:X4}\tSI: {2:X4}\tDI: {3:X4}",
                cpu.EU.Registers.SP, cpu.EU.Registers.BP, cpu.EU.Registers.SI, cpu.EU.Registers.DI));

            Console.WriteLine(string.Format("CS: {0:X4}\tDS: {1:X4}\tSS: {2:X4}\tES: {3:X4}",
                cpu.Bus.CS, cpu.Bus.DS, cpu.Bus.SS, cpu.Bus.ES));

            Console.WriteLine("\t\tTF DF IF OF SF ZF AF PF CF");

            Console.WriteLine(string.Format("\t\t{0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}  {8}",
                FlagToInt(cpu.EU.CondReg.TrapFlag),
                FlagToInt(cpu.EU.CondReg.DirectionFlag),
                FlagToInt(cpu.EU.CondReg.InterruptEnable),
                FlagToInt(cpu.EU.CondReg.OverflowFlag),
                FlagToInt(cpu.EU.CondReg.SignFlag),
                FlagToInt(cpu.EU.CondReg.ZeroFlag),
                FlagToInt(cpu.EU.CondReg.AuxCarryFlag),
                FlagToInt(cpu.EU.CondReg.ParityFlag),
                FlagToInt(cpu.EU.CondReg.CarryFlag)));

            Console.WriteLine();
            string dasm;
            int bytes = (int)Disassemble8086.DisassembleNext(cpu.Bus.GetNext6Bytes(), 0, 0, out dasm);
            Console.WriteLine("IP {0:X4}: {1}", cpu.Bus.IP,dasm);

            byte[] data = cpu.Bus.GetNextIPBytes(bytes);
            string opstring = "";
            for (int ii = 0; ii < bytes; ii++)
            {
                opstring += string.Format("{0:X2}", data[ii]);
                opstring += " ";
            }
            Console.WriteLine("Op Code: " + opstring);

            if (cpu.EU.Halted)
            {
                Console.WriteLine("Halted! Executed " + count.ToString() + " instructions");
                Console.WriteLine("Execution Time: " + sw.ElapsedMilliseconds.ToString());
            }

        }

        static void DisplayVRAM()
        {
            StringBuilder line;
            int addr = 0x8000;
            for(int row = 0; row < 25; row++)
            {
                line = new StringBuilder();
                for( int col=0; col < 80; col++)
                {
                    line.Append((char)cpu.Bus.GetPhysicalRAM(addr++));
                }
                Console.WriteLine(line.ToString());
            }
        }

        static void DisplayMenu()
        {
            Console.WriteLine("[Enter]:step [R]:run [B]:run to break [Q]:quit");
            int input;
            char ch = '0';
            try
            {
                input = Console.Read();
                ch = Convert.ToChar(input);
                if( input == 13)
                {
                    cpu.EU.NextInstruction();
                    input = Console.Read();  // throw away lf char
                }
                else if( ch == 'r' || ch == 'R')
                {
                    sw.Restart();
                    count = cpu.Run();
                    sw.Stop();
                }
                else if( ch == 'b' || ch == 'B')
                {
                    // flush CRLF
                    input = Console.Read();
                    input = Console.Read();
                    // loop until we hit our breakpoint
                    do
                    {
                        cpu.EU.NextInstruction();
                    } while (cpu.Bus.IP != 0x01a2);
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Exception!");
            }
            if (ch == 'Q' || ch == 'q')
                exit = true;

        }

        static int FlagToInt(bool flag)
        {
            if (flag)
                return 1;
            else
                return 0;
        }
    }
}
