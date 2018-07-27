﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_SUB
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
        public void Test28()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x28, 0xd8, /* SUB AL,BL */
                                      0x28, 0xd1 /* SUB CL,DL */
                        });

            cpu.EU.Registers.AL = 0x00;
            cpu.EU.Registers.BL = 0x00;
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.AL, "SUB (1) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SUB (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (1) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.CL, "SUB (2) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SUB (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (2) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (2) overflow flag failed");
        }

        [TestMethod]
        public void Test29()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x01, 0xd8, /* SUB AX,BX */
                                      0x01, 0xd1, /* SUB CX,DD */
                                      0x01, 0xe7, /* SUB DI,SP */
                                      0x01, 0x36, 0x15, 0x01 /* SUB [0115],SI */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "SUB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 80+80=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.CX, "SUB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000=8000.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8000, cpu.EU.Registers.DI, "SUB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SUB (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SUB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.Bus.SaveData(1, 0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.Bus.GetData(1, 0x0115), "SUB (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (4) overflow flag failed");

        }

        [TestMethod]
        public void Test2a()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x02, 0xd8, /* SUB BL,AL */
                                      0x02, 0xd1, /* SUB DL,CL */
                                      0x02, 0xe7, /* SUB AH,BH */
                                      0x02, 0x36, 0x15, 0x01 /* SUB DH,[0115] */
                        });

            cpu.EU.Registers.AL = 0xf7;
            cpu.EU.Registers.BL = 0x28;  // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.BL, "SUB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SUB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (1) overflow flag failed");

            cpu.EU.Registers.CL = 0x80;
            cpu.EU.Registers.DL = 0x80;  // 80+80=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DL, "SUB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (2) overflow flag failed");

            cpu.EU.Registers.BH = 0x40;
            cpu.EU.Registers.AH = 0x40;  // 40+40=80.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x80, cpu.EU.Registers.AH, "SUB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SUB (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SUB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SUB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (3) overflow flag failed");

            cpu.EU.Registers.DH = 0xff;
            cpu.Bus.SaveData(0, 0x0115, 0x01); // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DH, "SUB (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (4) sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "SUB (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (4) overflow flag failed");

        }

        [TestMethod]
        public void Test2b()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x03, 0xd8, /* SUB BX,AX */
                                      0x03, 0xd1, /* SUB DX,CX */
                                      0x03, 0xe7, /* SUB SP,DI */
                                      0x03, 0x36, 0x15, 0x01 /* SUB SI,[0115] */
                        });

            cpu.EU.Registers.AX = 0xf700;
            cpu.EU.Registers.BX = 0x2800;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.BX, "SUB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (1) overflow flag failed");

            cpu.EU.Registers.CX = 0x8000;
            cpu.EU.Registers.DX = 0x8000;  // 80+80=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.DX, "SUB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (2) overflow flag failed");

            cpu.EU.Registers.DI = 0x4000;
            cpu.EU.Registers.SP = 0x4000;  // 4000+4000=8000.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8000, cpu.EU.Registers.SP, "SUB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SUB (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SUB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (3) overflow flag failed");

            cpu.EU.Registers.SI = 0xff00;
            cpu.Bus.SaveData(1, 0x0115, 0x0100); // ff00+0100=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Registers.SI, "SUB (4) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (4) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (4) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (4) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (4) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (4) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (4) overflow flag failed");

        }


        [TestMethod]
        public void Test2c()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x04, 0x28, /* SUB AL,28H */
                                      0x04, 0x80, /* SUB AL,80H */
                                      0x04, 0x40 /* SUB BH,40H */
                        });

            cpu.EU.Registers.AL = 0xf7; // F7+28=11F.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f, cpu.EU.Registers.AL, "SUB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SUB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (1) overflow flag failed");

            cpu.EU.Registers.AL = 0x80;  // 80+80=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x00, cpu.EU.Registers.AL, "SUB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (2) overflow flag failed");

            cpu.EU.Registers.AL = 0x40; // 40+40=80.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x80, cpu.EU.Registers.AL, "SUB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SUB (3) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "SUB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SUB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (3) overflow flag failed");
        }

        [TestMethod]
        public void Test2d()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x05, 0x00, 0x28, /* SUB AX,2800H */
                                      0x05, 0x00, 0x80, /* SUB AX,8000H */
                                      0x05, 0x00, 0x40 /* SUB AX,4000H */
                        });

            cpu.EU.Registers.AX = 0xf700;  // F700+2800=11F00.  Carry=1, Parity=0, Zero=0, Sign=0, AuxCarry=0, Overflow=0
            cpu.NextInstruction();
            Assert.AreEqual(0x1f00, cpu.EU.Registers.AX, "SUB (1) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (1) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (1) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (1) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (1) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (1) auxcarry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "SUB (1) overflow flag failed");

            cpu.EU.Registers.AX = 0x8000;  // 80+80=10000.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x0000, cpu.EU.Registers.AX, "SUB (2) result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SUB (2) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (2) parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "SUB (2) zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "SUB (2) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (2) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (2) overflow flag failed");

            cpu.EU.Registers.AX = 0x4000;  // 4000+4000=8000.  Carry=0, Parity=0, Zero=0, Sign=1, AuxCarry=0, Overflow=1
            cpu.NextInstruction();
            Assert.AreEqual(0x8000, cpu.EU.Registers.AX, "SUB (3) result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SUB (3) carry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "SUB (3) parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "SUB (3) zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "SUB (3) sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.AuxCarryFlag, "SUB (3) auxcarry flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "SUB (3) overflow flag failed");

        }

        [TestMethod]
        public void Test83()
        {
            // Test Flags
            CPU cpu = GetCPU(new byte[] { 0x83, 0xf8, 0x63, /* CMP AX, 63 */
                                                0x83, 0xf8, 0x63 /* CMP AX, 63 */
                        });

            cpu.EU.Registers.AX = 0x1000;
            cpu.NextInstruction();

            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "CMP (1) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "CMP (1) zero flag failed");

            cpu.EU.Registers.AX = 0x0058;
            cpu.NextInstruction();

            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "CMP (2) carry flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "CMP (2) zero flag failed");
        }

        [TestMethod]
        public void TestFlags()
        {
            // Tests every combination of OF, SF, ZF, CF using boundary conditions
            SubtractionTest(0xff, 0xfe, 0x01, false, false, false, false);
            SubtractionTest(0x7e, 0xff, 0x7f, false, false, false, true);
            SubtractionTest(0xff, 0xff, 0x00, false, false, true, false);
            SubtractionTest(0xff, 0x7f, 0x80, false, true, false, false);
            SubtractionTest(0xfe, 0xff, 0xff, false, true, false, true);
            SubtractionTest(0xfe, 0x7f, 0x7f, true, false, false, false);
            SubtractionTest(0x7f, 0xff, 0x80, true, true, false, true);
        }

        public void SubtractionTest(byte a, byte b, byte expected_result, bool expected_of, bool expected_sf, bool expected_zf, bool expected_cf)
        {
            CPU cpu = GetCPU(new byte[] { 0x28, 0xd8 });  // sub al,bl
            cpu.EU.Registers.AL = a;
            cpu.EU.Registers.BL = b;
            cpu.NextInstruction();

            string fmt = string.Format("SUB Flag Test({0:X2}-{1:X2}) ", a, b);
            Assert.AreEqual(expected_result, cpu.EU.Registers.AL, fmt + " result failed");
            Assert.AreEqual(expected_of, cpu.EU.CondReg.OverflowFlag, fmt + " OF failed");
            Assert.AreEqual(expected_sf, cpu.EU.CondReg.SignFlag, fmt + " SF failed");
            Assert.AreEqual(expected_zf, cpu.EU.CondReg.ZeroFlag, fmt + " ZF failed");
            Assert.AreEqual(expected_cf, cpu.EU.CondReg.CarryFlag, fmt + " CF failed");
        }
    }
}
