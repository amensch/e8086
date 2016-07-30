using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_ADD
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
        public void Test00()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x00, 0xd8, /* ADD AL,BL */
                                      0x00, 0xd1, /* ADD CL,DL */
                                      0x00, 0xe7, /* ADD BH,AH */
                                      0x00, 0x36, 0x15, 0x01 /* ADD [0115],DH */
                        });

            cpu.EU.Registers.AL = 0xf7;
            cpu.EU.Registers.BL = 0x28;  // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.AL, "ADD (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADD (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;  // 80+80=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.CL, "ADD (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");

            cpu.EU.Registers.BH = 0x40;
            cpu.EU.Registers.AH = 0x40;  // 40+40=80.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x80, cpu.EU.Registers.BH, "ADD (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADD (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADD (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (3) overflow flag failed");

            cpu.EU.Registers.DH = 0xff;
            cpu.EU.Bus.SaveData8(0x0115, 0x01); // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Bus.GetData8(0x0115), "ADD (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (4) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "ADD (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (4) overflow flag failed");

        }

        [TestMethod]
        public void Test01()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x01, 0xd8, /* ADD AX,BX */
                                      0x01, 0xd1, /* ADD CX,DD */
                                      0x01, 0xe7, /* ADD DI,SP */
                                      0x01, 0x36, 0x15, 0x01 /* ADD [0115],SI */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "ADD (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 80+80=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.CX, "ADD (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000=8000.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8000, cpu.EU.Registers.DI, "ADD (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADD (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.EU.Bus.SaveData16(0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Bus.GetData16(0x0115), "ADD (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (4) overflow flag failed");

        }

        [TestMethod]
        public void Test02()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x02, 0xd8, /* ADD BL,AL */
                                      0x02, 0xd1, /* ADD DL,CL */
                                      0x02, 0xe7, /* ADD AH,BH */
                                      0x02, 0x36, 0x15, 0x01 /* ADD DH,[0115] */
                        });

            cpu.EU.Registers.AL = 0xf7;
            cpu.EU.Registers.BL = 0x28;  // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.BL, "ADD (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADD (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;  // 80+80=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DL, "ADD (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");

            cpu.EU.Registers.BH = 0x40;
            cpu.EU.Registers.AH = 0x40;  // 40+40=80.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x80, cpu.EU.Registers.AH, "ADD (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADD (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADD (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (3) overflow flag failed");

            cpu.EU.Registers.DH = 0xff;
            cpu.EU.Bus.SaveData8(0x0115, 0x01); // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DH, "ADD (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (4) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "ADD (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (4) overflow flag failed");

        }

        [TestMethod]
        public void Test03()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x03, 0xd8, /* ADD BX,AX */
                                      0x03, 0xd1, /* ADD DX,CX */
                                      0x03, 0xe7, /* ADD SP,DI */
                                      0x03, 0x36, 0x15, 0x01 /* ADD SI,[0115] */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.BX, "ADD (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 80+80=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DX, "ADD (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000=8000.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8000, cpu.EU.Registers.SP, "ADD (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADD (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.EU.Bus.SaveData16(0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Registers.SI, "ADD (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (4) overflow flag failed");

        }


        [TestMethod]
        public void Test04()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x04, 0x28, /* ADD AL,28H */
                                      0x04, 0x80, /* ADD AL,80H */
                                      0x04, 0x40 /* ADD BH,40H */
                        });

            cpu.EU.Registers.AL = 0xf7; // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.AL, "ADD (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADD (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (1) overflow flag failed");

            cpu.EU.Registers.AL = 0x80;  // 80+80=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.AL, "ADD (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");

            cpu.EU.Registers.AL = 0x40; // 40+40=80.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x80, cpu.EU.Registers.AL, "ADD (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "ADD (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADD (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (3) overflow flag failed");
        }

        [TestMethod]
        public void Test05()
        {
            // Test Flags
            i8086CPU cpu = GetCPU(new byte[] { 0x05, 0x00, 0x28, /* ADD AX,2800H */
                                      0x05, 0x00, 0x80, /* ADD AX,8000H */
                                      0x05, 0x00, 0x40 /* ADD AX,4000H */
                        });

            cpu.EU.Registers.AX = 0xf700;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "ADD (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (1) overflow flag failed");

            cpu.EU.Registers.AX = 0x8000;  // 80+80=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Registers.AX, "ADD (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");

            cpu.EU.Registers.AX = 0x4000;  // 4000+4000=8000.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8000, cpu.EU.Registers.AX, "ADD (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "ADD (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (3) overflow flag failed");

        }

        [TestMethod]
        public void Test80()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0x80, 0xc1, 0x80 });
            cpu.EU.Registers.CL = 0x80;  // 80+80=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.CL, "ADD (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (1) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (1) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (1) overflow flag failed");


            cpu = GetCPU(new byte[] { 0x80, 0x06, 0x15, 0x01, 0x80 }); /* SBB [0115],80h */
            cpu.EU.Bus.SaveData8(0x0115, 0x80);  // 80+80=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Bus.GetData8(0x0115), "ADD (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");

            cpu = GetCPU(new byte[] { 0x81, 0x06, 0x15, 0x01, 0x80, 0x2e }); /* SBB [0115],2e80h */
            cpu.EU.Bus.SaveData16(0x0115, 0x1234);  // 1234+2e80=40b4.  Carry=0, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x40b4, cpu.EU.Bus.GetData16(0x0115), "ADD (2) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ADD (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "ADD (2) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "ADD (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "ADD (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "ADD (2) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "ADD (2) overflow flag failed");
        }

    }
}
