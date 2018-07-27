using System;
using System.Text;
using System.Collections.Generic;
using KDS.e8086;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KDS.e8086UnitTests
{
    /// <summary>
    /// Summary description for EUTest_POST_Tests
    /// </summary>
    [TestClass]
    public class EUTest_POST_Tests
    {
        private CPU GetCPU(byte[] program)
        {
            CPU cpu = new CPU();
            cpu.Boot(program);
            cpu.Bus.DS = 0x0000;
            cpu.Bus.SS = 0x0000;
            cpu.Bus.ES = 0x0000;
            return cpu;
        }

        public EUTest_POST_Tests()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void Test_POST_1()
        {
            CPU cpu = GetCPU(new byte[] {
                                        // Test 1
                                        0xb4, 0xd5,  /* MOV AH, 0xD5 */
                                        0x9e,         /* SAHF */
                                        // Test 2
                                        0x9f,         /* LAHF */
                                        0xb1, 0x05,   /* MOV CL,5 */
                                        0xd2, 0xec,   /* SHR AH, CL */
                                        // Test 3
                                        0xb0, 0x40,   /* MOV AL, 40 */
                                        0xd0, 0xe0,   /* SHL AL, 1*/ 
                                        // Test 4
                                        0x32, 0xe4,   /* XOR ah, ah */
                                        0x9e,         /* SAHF */
                                        // Test 5
                                        0x9f,         /* LAHF */
                                        0xb1, 0x05,   /* MOV CL,5 */
                                        0xd2, 0xec,   /* SHL AH,CL */
                                        // Test 6
                                        0xd0, 0xe4    /* SHL AH,1 */
                        });

            cpu.NextInstruction();
            cpu.NextInstruction();
            Assert.AreEqual(true, cpu.EU.CondReg.ParityFlag, "POST 1: parity flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "POST 1: zero flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.SignFlag, "POST 1: sign flag failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "POST 1: carry flag failed");

            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();
            Assert.AreEqual(true, cpu.EU.CondReg.AuxCarryFlag, "POST 2: aux carry flag failed");

            cpu.NextInstruction();
            cpu.NextInstruction();
            Assert.AreEqual(true, cpu.EU.CondReg.OverflowFlag, "POST 3: overflow flag failed");

            cpu.NextInstruction();
            cpu.NextInstruction();
            Assert.AreEqual(false, cpu.EU.CondReg.ParityFlag, "POST 4: parity flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.ZeroFlag, "POST 4: zero flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.SignFlag, "POST 4: sign flag failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "POST 4: carry flag failed");

            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "POST 5: carry flag failed");

            cpu.NextInstruction();
            Assert.AreEqual(false, cpu.EU.CondReg.OverflowFlag, "POST 6: overflow flag failed");
        }

        [TestMethod]
        public void Test_POST_2()
        {
            // writes into each register

            CPU cpu = GetCPU(new byte[] {
                                        0xb8, 0xff, 0xff,   // mov ax,0xffff
                                        0xf9,               // stc
                   /* C8 */             0x8e, 0xd8,         // mov ds, ax
                                        0x8c, 0xdb,         // mov bx, ds
                                        0x8e, 0xc3,         // mov es, bx
                                        0x8c, 0xc1,         // mov cx, es
                                        0x8e, 0xd1,         // mov ss, cx
                                        0x8c, 0xd2,         // mov dx, ss
                                        0x8b, 0xe2,         // mov sp, dx
                                        0x8b, 0xec,         // mov bp, sp
                                        0x8b, 0xf5,         // mov si, bp
                                        0x8b, 0xfe,         // mov di, si
                                        0x73, 0x07,         // jnc c9
                                        0x33, 0xc7,         // xor ax, di
                                        0x75, 0x07,         // jnz err
                                        0xf8,               // clc
                                        0xeb, 0xe3,         // jmp c8
                  /* C9 */              0x0b, 0xc7,         // or
                                        0xf4 //hlt
            });

            cpu.Run();
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "POST 2: zero flag failed");

        }

        [TestMethod]
        public void Test_SumDigitsTestProgram()
        {
            Test_SumDigitsTest2(0x54, 0x9);
            Test_SumDigitsTest2(0x60, 0x6);
            Test_SumDigitsTest2(0x50, 0x5);
            Test_SumDigitsTest2(0x55, 0x0a);
            Test_SumDigitsTest2(0x2,  0x2);
            Test_SumDigitsTest2(0x45, 0x9);
            Test_SumDigitsTest2(0x12, 0x3);
            Test_SumDigitsTest2(0x99, 0x12);
            Test_SumDigitsTest2(0x75, 0x0c);
        }

        public void Test_SumDigitsTest2(byte src, byte dest)
        {
            // take the number in memory 2050, find the sum of the digits and store in 2051

            CPU cpu = GetCPU(new byte[] {
                                        0xa0, 0x02, 0x08,                   // mov al,[2050]
                                        0x88, 0xc4,                         // mov ah, al
                                        0xb1, 0x04,                         // mov cl, 4
                                        0x24, 0x0f,                         // and al, 0x0f
                                        0xd2, 0xc4,                         // rol ah, cl
                                        0x80, 0xe4, 0x0f,                   // and ah, 0x0f
                                        0x00, 0xe0,                         // add al, ah
                                        0xa2, 0x03, 0x08, 0x00, 0x00,       // mov [2051], al
                                        0xf4 //hlt
            });

            cpu.Bus.SaveData(0, 2050, src);
            cpu.Run();
            byte data = (byte)cpu.Bus.GetData(0, 2051);
            Assert.AreEqual(dest, data, "Sum test failed " + src.ToString() + " expected answer " + dest.ToString() + " actual answer " + data.ToString());
        }
    }
}
