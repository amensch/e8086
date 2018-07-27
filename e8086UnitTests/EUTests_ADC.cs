using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_ADC
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
        public void Test10()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x10, 0xd8, /* ADC AL,BL */
                                      0x10, 0xd1, /* ADC CL,DL */
                                      0x10, 0xe7, /* ADC BH,AH */
                                      0x10, 0x36, 0x15, 0x01 /* ADC [0115],DH */
                        });

            cpu.EU.Registers.AL = 0xf7;
            cpu.EU.Registers.BL = 0x28;  // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.AL, "ADC (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;  // 80+80+1=101.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.CL, "ADC (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (2) overflow flag failed");

            cpu.EU.Registers.BH = 0x40;
            cpu.EU.Registers.AH = 0x40;  // 40+40+1=81.  Carry=0, Parity=1, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x81, cpu.EU.Registers.BH, "ADC (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADC (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADC (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (3) overflow flag failed");

            cpu.EU.Registers.DH = 0xff;
            cpu.Bus.SaveByte(0x0115, 0x01); // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.Bus.GetData(0, 0x0115), "ADC (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADC (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (4) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "ADC (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (4) overflow flag failed");

        }

        [TestMethod]
        public void Test11()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x11, 0xd8, /* ADC AX,BX */
                                      0x11, 0xd1, /* ADC CX,DD */
                                      0x11, 0xe7, /* ADC DI,SP */
                                      0x11, 0x36, 0x15, 0x01 /* ADC [0115],SI */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "ADC (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 8000+8000+1=10001.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.CX, "ADC (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000+1=8001.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8001, cpu.EU.Registers.DI, "ADC (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADC (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADC (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.Bus.SaveWord(0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.Bus.GetData(1, 0x0115), "ADC (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADC (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (4) overflow flag failed");

        }

        [TestMethod]
        public void Test12()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x12, 0xd8, /* ADC BL,AL */
                                      0x12, 0xd1, /* ADC DL,CL */
                                      0x12, 0xe7, /* ADC AH,BH */
                                      0x12, 0x36, 0x15, 0x01 /* ADC DH,[0115] */
                        });

            cpu.EU.Registers.AL = 0xf7;
            cpu.EU.Registers.BL = 0x28;  // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.BL, "ADC (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;  // 80+80+1=101.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.DL, "ADC (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (2) overflow flag failed");

            cpu.EU.Registers.BH = 0x40;
            cpu.EU.Registers.AH = 0x40;  // 40+40+1=81.  Carry=0, Parity=1, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x81, cpu.EU.Registers.AH, "ADC (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADC (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADC (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (3) overflow flag failed");

            cpu.EU.Registers.DH = 0xff;
            cpu.Bus.SaveByte(0x0115, 0x01); // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DH, "ADC (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADC (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (4) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "ADC (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (4) overflow flag failed");

        }

        [TestMethod]
        public void Test13()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x13, 0xd8, /* ADC BX,AX */
                                      0x13, 0xd1, /* ADC DX,CX */
                                      0x13, 0xe7, /* ADC SP,DI */
                                      0x13, 0x36, 0x15, 0x01 /* ADC SI,[0115] */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.BX, "ADC (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 80+80+1=10001.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.DX, "ADC (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000+1=8001.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8001, cpu.EU.Registers.SP, "ADC (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADC (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADC (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.Bus.SaveWord(0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Registers.SI, "ADC (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADC (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (4) overflow flag failed");

        }


        [TestMethod]
        public void Test14()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x14, 0x28, /* ADC AL,28H */
                                      0x14, 0x80, /* ADC AL,80H */
                                      0x14, 0x40 /* ADC BH,40H */
                        });

            cpu.EU.Registers.AL = 0xf7; // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.AL, "ADC (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (1) overflow flag failed");

            cpu.EU.Registers.AL = 0x80;  // 80+80+1=101.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x01, cpu.EU.Registers.AL, "ADC (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (2) overflow flag failed");

            cpu.EU.Registers.AL = 0x40; // 40+40+1=81.  Carry=0, Parity=1, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x81, cpu.EU.Registers.AL, "ADC (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADC (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADC (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (3) overflow flag failed");
        }

        [TestMethod]
        public void Test15()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x15, 0x00, 0x28, /* ADC AX,2800H */
                                      0x15, 0x00, 0x80, /* ADC AX,8000H */
                                      0x15, 0x00, 0x40 /* ADC AX,4000H */
                        });

            cpu.EU.Registers.AX = 0xf700;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "ADC (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADC (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADC (1) overflow flag failed");

            cpu.EU.Registers.AX = 0x8000;  // 80+80+1=10001.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x0001, cpu.EU.Registers.AX, "ADC (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADC (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADC (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (2) overflow flag failed");

            cpu.EU.Registers.AX = 0x4000;  // 4000+4000+1=8001.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8001, cpu.EU.Registers.AX, "ADC (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADC (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADC (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADC (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADC (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADC (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADC (3) overflow flag failed");

        }
    }
}
