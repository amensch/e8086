using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_INCandDEC
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
        public void TestINC()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x40, 0x41, 0x42, 0x43 });

            cpu.EU.Registers.AX = 0x000f;
            cpu.NextInstruction();
            Assert.AreEqual(0x0010, cpu.EU.Registers.AX, "INC (1) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "INC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "INC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "INC (1) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "INC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "INC (1) overflow flag failed");

            cpu.EU.Registers.CX = 0xff01;
            cpu.NextInstruction();
            Assert.AreEqual(0xff02, cpu.EU.Registers.CX, "INC (2) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "INC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "INC (2) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "INC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "INC (2) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "INC (2) overflow flag failed");

            cpu.EU.Registers.DX = 0xffff;
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Registers.DX, "INC (3) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "INC (3) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "INC (3) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "INC (3) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "INC (3) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "INC (3) overflow flag failed");
        }


        [TestMethod]
        public void TestDEC()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x4c, 0x4d, 0x4e, 0x4f });

            cpu.EU.Registers.SP = 0x000f;
            cpu.NextInstruction();
            Assert.AreEqual(0x000e, cpu.EU.Registers.SP, "DEC (1) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "DEC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "DEC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "DEC (1) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "DEC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "DEC (1) overflow flag failed");

            cpu.EU.Registers.BP = 0xff01;
            cpu.NextInstruction();
            Assert.AreEqual(0xff00, cpu.EU.Registers.BP, "DEC (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "DEC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "DEC (2) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "DEC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "DEC (2) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "DEC (2) overflow flag failed");

            cpu.EU.Registers.SI = 0x0000;
            cpu.NextInstruction();
            Assert.AreEqual(0xffff, cpu.EU.Registers.SI, "DEC (3) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "DEC (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "DEC (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "DEC (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "DEC (3) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "DEC (3) overflow flag failed");
        }
    }
}
