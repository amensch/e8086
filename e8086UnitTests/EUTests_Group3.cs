using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_Group3
    {

        private i8086CPU GetCPU(byte[] program)
        {
            i8086CPU cpu = new i8086CPU();
            cpu.Boot(program);
            cpu.EU.Bus.DS = 0x2000;
            cpu.EU.Bus.SS = 0x4000;
            cpu.EU.Bus.ES = 0x6000;
            return cpu;
        }

        [TestMethod]
        public void TestMUL()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xf6, 0xe3 }); /* MUL BL */

            cpu.EU.Registers.AL = 0xc8;
            cpu.EU.Registers.BL = 0x04;
            cpu.NextInstruction();

            Assert.AreEqual(0x0320, cpu.EU.Registers.AX, "MUL AX result failed");
        }

        [TestMethod]
        public void TestIMUL()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xf6, 0xeb }); /* IMUL BL */

            cpu.EU.Registers.AL = 0xfe;
            cpu.EU.Registers.BL = 0xfc;
            cpu.NextInstruction();

            Assert.AreEqual(0x08, cpu.EU.Registers.AX, "IMUL AX result failed");
        }

        [TestMethod]
        public void TestDIV()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xf6, 0xf3 }); /* DIV BL */

            cpu.EU.Registers.AX = 0xcb;
            cpu.EU.Registers.BL = 0x04;  
            cpu.NextInstruction();

            Assert.AreEqual(0x32, cpu.EU.Registers.AL, "DIV AL result failed");
            Assert.AreEqual(0x03, cpu.EU.Registers.AH, "DIV AH result failed");
        }

        [TestMethod]
        public void TestIDIV()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xf6, 0xfb }); /* IDIV BL */

            cpu.EU.Registers.AX = 0xff35;
            cpu.EU.Registers.BL = 0x04;
            cpu.NextInstruction();

            Assert.AreEqual(0xce, cpu.EU.Registers.AL, "IDIV AL result failed");
            Assert.AreEqual(0xfd, cpu.EU.Registers.AH, "IDIV AH result failed");
        }
    }
}
