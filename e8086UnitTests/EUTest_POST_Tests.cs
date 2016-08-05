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
        private i8086CPU GetCPU(byte[] program)
        {
            i8086CPU cpu = new i8086CPU();
            cpu.Boot(program);
            cpu.EU.Bus.DS = 0x0000;
            cpu.EU.Bus.SS = 0x0000;
            cpu.EU.Bus.ES = 0x0000;
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
            i8086CPU cpu = GetCPU(new byte[] {
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

            i8086CPU cpu = GetCPU(new byte[] {
                                        0xb8, 0xff, 0xff,   // mov ax,0xffff
                                        0xf9,               // stc
                   /* C8 */             0x8e, 0xd8,         // mov ds, ax
                                        0x8c, 0xdb,         // mov bx, ds
                                        0x8e, 0xc3,         // mov es, bx
                                        0x8c, 0xc1,
                                        0x8e, 0xd1,
                                        0x8c, 0xd2,
                                        0x8b, 0xe2,
                                        0x8b, 0xec,
                                        0x8b, 0xf5,
                                        0x8b, 0xfe,
                                        0x73, 0x07,         // jnc c9
                                        0x33, 0xc7,
                                        0x75, 0x07,         // jnz err
                                        0xf8,               // clc
                                        0xeb, 0xe3,         // jmp c8
                  /* C9 */              0x0b, 0xc7,
                                        0xf4 //hlt
            });

            cpu.Run();
            Assert.AreEqual(true, cpu.EU.CondReg.ZeroFlag, "POST 2: zero flag failed");

        }
    }
}
