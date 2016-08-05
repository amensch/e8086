using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_AdjustTests
    {
        private i8086CPU GetCPU(byte[] program)
        {
            i8086CPU cpu = new i8086CPU();
            cpu.Boot(program);
            cpu.Bus.DS = 0x2000;
            cpu.Bus.SS = 0x4000;
            cpu.Bus.ES = 0x6000;
            return cpu;
        }

        [TestMethod]
        public void Test_DAA()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x00, 0xd8, /* ADD AL,BL */
                                      0x27 /* DAA */ });

            cpu.EU.Registers.AL = 0x79;
            cpu.EU.Registers.BL = 0x35;

            cpu.NextInstruction();
            Assert.AreEqual(0xae, cpu.EU.Registers.AL, "ADD result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD overflow flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADD sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADD parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD carry flag failed");

            cpu.NextInstruction();
            Assert.AreEqual(0x14, cpu.EU.Registers.AL, "DAA (1) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "DAA (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "DAA (1) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "DAA (1) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "DAA (1) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "DAA (1) carry flag failed");

        }

        [TestMethod]
        public void Test_DAS()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x2f });

            cpu.EU.Registers.AL = 0xff;

            cpu.NextInstruction();
            Assert.AreEqual(0x99, cpu.EU.Registers.AL, "DAS result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "DAS overflow flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "DAS sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "DAS zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "DAS auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "DAS parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "DAS carry flag failed");

        }

        [TestMethod]
        public void Test_AAA()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x37 });

            cpu.EU.Registers.AX = 0x0f;

            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.AH, "AAA AH result failed");
            Assert.AreEqual(0x05, cpu.EU.Registers.AL, "AAA AL result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "AAA auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "AAA carry flag failed");
        }

        [TestMethod]
        public void Test_AAS()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x3f });

            cpu.EU.Registers.AX = 0x2ff;

            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.AH, "AAA AH result failed");
            Assert.AreEqual(0x09, cpu.EU.Registers.AL, "AAA AL result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "AAA auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "AAA carry flag failed");
        }

        [TestMethod]
        public void Test_AAM()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xd4 });

            cpu.EU.Registers.AL = 0x0f;

            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.AH, "AAM AH result failed");
            Assert.AreEqual(0x05, cpu.EU.Registers.AL, "AAM AL result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "AAM sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "AAM zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "AAM parity flag failed");
        }

        [TestMethod]
        public void Test_AAD()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xd5 });

            cpu.EU.Registers.AX = 0x0105;

            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.AH, "AAM AH result failed");
            Assert.AreEqual(0x0f, cpu.EU.Registers.AL, "AAM AL result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "AAM sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "AAM zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "AAM parity flag failed");
        }

        [TestMethod]
        public void Test_CBW()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x98 });

            cpu.EU.Registers.AX = 0x00fb;
            cpu.NextInstruction();
            Assert.AreEqual(0xfffb, cpu.EU.Registers.AX, "CBW result failed");
        }

        [TestMethod]
        public void Test_CWD()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x99 });

            cpu.EU.Registers.AX = 0xfffb;
            cpu.NextInstruction();
            Assert.AreEqual(0xfffb, cpu.EU.Registers.AX, "CWD result failed");
            Assert.AreEqual(0Xffff, cpu.EU.Registers.DX, "CWD result failed");
        }

        [TestMethod]
        public void Test_LoadAndStoreFlags()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x9f, 0x9e });

            cpu.EU.CondReg.Register = 0x15d7;
            cpu.NextInstruction();
            Assert.AreEqual(0xd7, cpu.EU.Registers.AH, "LAHF result failed");
            Assert.AreEqual(0x15d7, cpu.EU.CondReg.Register, "LAHF result failed");

            cpu.EU.CondReg.Register = 0xf92e;
            cpu.NextInstruction();
            Assert.AreEqual(0xd7, cpu.EU.Registers.AH, "SAHF result failed");
            Assert.AreEqual(0xf9d7, cpu.EU.CondReg.Register, "SAHF result failed");
        }
    }
}
