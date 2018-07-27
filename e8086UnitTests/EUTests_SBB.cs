using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_SBB
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
        public void Test18()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x10, 0xd8, /* SBB AL,BL */
                                      0x10, 0xd1, /* SBB CL,DL */
                                      0x10, 0xe7, /* SBB BH,AH */
                                      0x10, 0x36, 0x15, 0x01 /* SBB [0115],DH */
                        });

            cpu.EU.Registers.AL = 0xf7;
            cpu.EU.Registers.BL = 0x28;  // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.AL, "SBB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;  // 80+80+1=101.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.CL, "SBB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (2) overflow flag failed");

            cpu.EU.Registers.BH = 0x40;
            cpu.EU.Registers.AH = 0x40;  // 40+40+1=81.  Carry=0, Parity=1, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x81, cpu.EU.Registers.BH, "SBB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SBB (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SBB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (3) overflow flag failed");

            cpu.EU.Registers.DH = 0xff;
            cpu.Bus.SaveData(0, 0x0115, 0x01); // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.Bus.GetData(0, 0x0115), "SBB (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SBB (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (4) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "SBB (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (4) overflow flag failed");

        }

        [TestMethod]
        public void Test19()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x11, 0xd8, /* SBB AX,BX */
                                      0x11, 0xd1, /* SBB CX,DD */
                                      0x11, 0xe7, /* SBB DI,SP */
                                      0x11, 0x36, 0x15, 0x01 /* SBB [0115],SI */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "SBB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 8000+8000+1=10001.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.CX, "SBB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000+1=8001.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8001, cpu.EU.Registers.DI, "SBB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SBB (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SBB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.Bus.SaveData(1, 0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.Bus.GetData(1, 0x0115), "SBB (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SBB (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (4) overflow flag failed");

        }

        [TestMethod]
        public void Test1a()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x12, 0xd8, /* SBB BL,AL */
                                      0x12, 0xd1, /* SBB DL,CL */
                                      0x12, 0xe7, /* SBB AH,BH */
                                      0x12, 0x36, 0x15, 0x01 /* SBB DH,[0115] */
                        });

            cpu.EU.Registers.AL = 0xf7;
            cpu.EU.Registers.BL = 0x28;  // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.BL, "SBB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;  // 80+80+1=101.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.DL, "SBB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (2) overflow flag failed");

            cpu.EU.Registers.BH = 0x40;
            cpu.EU.Registers.AH = 0x40;  // 40+40+1=81.  Carry=0, Parity=1, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x81, cpu.EU.Registers.AH, "SBB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SBB (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SBB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (3) overflow flag failed");

            cpu.EU.Registers.DH = 0xff;
            cpu.Bus.SaveData(0, 0x0115, 0x01); // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DH, "SBB (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SBB (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (4) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "SBB (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (4) overflow flag failed");

        }

        [TestMethod]
        public void Test1b()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x13, 0xd8, /* SBB BX,AX */
                                      0x13, 0xd1, /* SBB DX,CX */
                                      0x13, 0xe7, /* SBB SP,DI */
                                      0x13, 0x36, 0x15, 0x01 /* SBB SI,[0115] */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.BX, "SBB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 80+80+1=10001.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.DX, "SBB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000+1=8001.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8001, cpu.EU.Registers.SP, "SBB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SBB (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SBB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.Bus.SaveData(1, 0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Registers.SI, "SBB (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SBB (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (4) overflow flag failed");

        }


        [TestMethod]
        public void Test1c()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x14, 0x28, /* SBB AL,28H */
                                      0x14, 0x80, /* SBB AL,80H */
                                      0x14, 0x40 /* SBB BH,40H */
                        });

            cpu.EU.Registers.AL = 0xf7; // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.AL, "SBB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (1) overflow flag failed");

            cpu.EU.Registers.AL = 0x80;  // 80+80+1=101.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.AL, "SBB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (2) overflow flag failed");

            cpu.EU.Registers.AL = 0x40; // 40+40+1=81.  Carry=0, Parity=1, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x81, cpu.EU.Registers.AL, "SBB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SBB (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SBB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (3) overflow flag failed");
        }

        [TestMethod]
        public void Test1d()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x15, 0x00, 0x28, /* SBB AX,2800H */
                                      0x15, 0x00, 0x80, /* SBB AX,8000H */
                                      0x15, 0x00, 0x40 /* SBB AX,4000H */
                        });

            cpu.EU.Registers.AX = 0xf700;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "SBB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SBB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SBB (1) overflow flag failed");

            cpu.EU.Registers.AX = 0x8000;  // 80+80+1=10001.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x0001, cpu.EU.Registers.AX, "SBB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SBB (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SBB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (2) overflow flag failed");

            cpu.EU.Registers.AX = 0x4000;  // 4000+4000+1=8001.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8001, cpu.EU.Registers.AX, "SBB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SBB (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SBB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SBB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SBB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SBB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SBB (3) overflow flag failed");

        }
    }
}
