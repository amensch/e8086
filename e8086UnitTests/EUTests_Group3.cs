using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_Group3
    {

        private CPU GetCPU(byte[] program)
        {
            CPU cpu = new CPU();
            cpu.Boot(program);
            cpu.Bus.DS = 0x2000;
            cpu.Bus.SS = 0x4000;
            cpu.Bus.ES = 0x6000;
            return cpu;
        }

        [TestMethod]
        public void TestMUL()
        {
            CPU cpu = GetCPU(new byte[] { 0xf6, 0xe3 }); /* MUL BL */

            cpu.EU.Registers.AL = 0xc8;
            cpu.EU.Registers.BL = 0x04;
            cpu.NextInstruction();

            Assert.AreEqual(0x0320, cpu.EU.Registers.AX, "MUL AX result failed");

            cpu = GetCPU(new byte[] { 0xf7, 0xe3 }); /* MUL BX */

            cpu.EU.Registers.AX = 0x2000;
            cpu.EU.Registers.BX = 0x0100;
            cpu.NextInstruction();
            Assert.AreEqual(0x0020, cpu.EU.Registers.DX, "MUL 2 DX result failed");
            Assert.AreEqual(0x0000, cpu.EU.Registers.AX, "MUL 2 AX result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "MUL 2 CF failed");
        }

        [TestMethod]
        public void TestIMUL()
        {
            CPU cpu = GetCPU(new byte[] { 0xf6, 0xeb }); /* IMUL BL */

            cpu.EU.Registers.AL = 0xfe;
            cpu.EU.Registers.BL = 0xfc;
            cpu.NextInstruction();

            Assert.AreEqual(0x08, cpu.EU.Registers.AX, "IMUL AX result failed");

            cpu = GetCPU(new byte[] { 0xf6, 0xeb }); /* IMUL BL */
            cpu.EU.Registers.AL = 48;
            cpu.EU.Registers.BL = 4;
            cpu.NextInstruction();

            Assert.AreEqual(0xc0, cpu.EU.Registers.AX, "IMUL 2 AX result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "IMUL 2 OF failed");

            cpu = GetCPU(new byte[] { 0xf6, 0xeb }); /* IMUL BL */
            cpu.EU.Registers.AL = 0xfc; // -4
            cpu.EU.Registers.BL = 4;
            cpu.NextInstruction();

            Assert.AreEqual(0xfff0, cpu.EU.Registers.AX, "IMUL 3 AX result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "IMUL 3 OF failed");

            cpu = GetCPU(new byte[] { 0xf7, 0xeb }); /* IMUL BX */
            cpu.EU.Registers.AX = 48; 
            cpu.EU.Registers.BX = 4;
            cpu.NextInstructionDebug();

            Assert.AreEqual(0x00, cpu.EU.Registers.DX, "IMUL 4 DX result failed");
            Assert.AreEqual(0xc0, cpu.EU.Registers.AX, "IMUL 4 AX result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "IMUL 4 OF failed");
        }

        [TestMethod]
        public void TestDIV()
        {
            CPU cpu = GetCPU(new byte[] { 0xf6, 0xf3 }); /* DIV BL */

            cpu.EU.Registers.AX = 0xcb;
            cpu.EU.Registers.BL = 0x04;  
            cpu.NextInstruction();

            Assert.AreEqual(0x32, cpu.EU.Registers.AL, "DIV AL result failed");
            Assert.AreEqual(0x03, cpu.EU.Registers.AH, "DIV AH result failed");

            cpu = GetCPU(new byte[] { 0xf7, 0xf3 }); /* DIV BX */
            cpu.EU.Registers.DX = 0;
            cpu.EU.Registers.AX = 5;
            cpu.EU.Registers.BX = 2;
            cpu.NextInstructionDebug();

            Assert.AreEqual(0x02, cpu.EU.Registers.AX, "DIV 2 AX result failed");
            Assert.AreEqual(0x01, cpu.EU.Registers.DX, "DIV 2 DX result failed");
        }

        [TestMethod]
        public void TestIDIV()
        {
            CPU cpu = GetCPU(new byte[] { 0xf6, 0xfb }); /* IDIV BL */

            cpu.EU.Registers.AX = 0xffd0; // -48
            cpu.EU.Registers.BL = 0x05;   // 5

            // result AL = -9, AH = -3
            cpu.NextInstruction();

            Assert.AreEqual(0xf7, cpu.EU.Registers.AL, "IDIV AL result failed");
            Assert.AreEqual(0xfd, cpu.EU.Registers.AH, "IDIV AH result failed");

            cpu = GetCPU(new byte[] { 0xf6, 0xfb }); /* IDIV BL */

            cpu.EU.Registers.AX = 0xff35; // -203
            cpu.EU.Registers.BL = 0x04;   // 4
            cpu.NextInstruction();

            //AL = -50, AH = -3
            Assert.AreEqual(0xce, cpu.EU.Registers.AL, "IDIV 2 AL result failed");
            Assert.AreEqual(0xfd, cpu.EU.Registers.AH, "IDIV 2 AH result failed");

            cpu = GetCPU(new byte[] { 0xf7, 0xfb }); /* IDIV BX */
            cpu.EU.Registers.DX = 0xffff;  // sign extend AX
            cpu.EU.Registers.AX = 0xec78;  // -5000
            cpu.EU.Registers.BX = 256;
            cpu.NextInstructionDebug();

            // AX = -19, DX = -136
            Assert.AreEqual(0xffed, cpu.EU.Registers.AX, "IDIV 3 AX result failed");
            Assert.AreEqual(0xff78, cpu.EU.Registers.DX, "IDIV 3 DX result failed");
        }
    }
}
