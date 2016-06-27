using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    /// <summary>
    /// Summary description for ExecutionUnitTests
    /// </summary>
    [TestClass]
    public class ExecutionUnitTests
    {
        public ExecutionUnitTests()
        {
            //
            // TODO: Add constructor logic here
            //
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestAAA()
        {
            i8086CPU cpu = new i8086CPU();
        }

        [TestMethod]
        public void Test88()
        {
            i8086CPU cpu = new i8086CPU();
            byte value8;

            cpu.Boot(new byte[] { 0x88, 0x4f, 0x10 } /* MOV [bx+10],CL */);
            value8 = 0x2f;

            cpu.EU.Registers.CL = value8;
            cpu.NextInstruction();

            Assert.AreEqual(value8, cpu.EU.Bus.GetData8(cpu.EU.Registers.BX + 0x10), "Instruction 0x88 failed");

            cpu.Boot(new byte[] { 0x88, 0x8f, 0x10, 0x00 } /* MOV [bx+10],CL using 16 bit displacement */);
            value8 = 0x2f;

            cpu.EU.Registers.CL = value8;
            cpu.NextInstruction();

            Assert.AreEqual(value8, cpu.EU.Bus.GetData8(cpu.EU.Registers.BX + 0x10), "Instruction 0x88 failed");

        }

        [TestMethod]
        public void Test89()
        {
            i8086CPU cpu = new i8086CPU();

            cpu.Boot(new byte[] { 0x89, 0xd8, 0x10 } /* MOV ax,bx */);

            cpu.EU.Registers.AX = 0x11ab;
            cpu.EU.Registers.BX = 0x22fe;
            cpu.NextInstruction();

            Assert.AreEqual(0x22fe, cpu.EU.Registers.AX, "Instruction 0x89 failed");
        }

        [TestMethod]
        public void Test8a()
        {
            i8086CPU cpu = new i8086CPU();
            byte value8;

            cpu.Boot(new byte[] { 0x8a, 0x4f, 0x10 } /* MOV CL, [BX+10] */);
            value8 = 0x2f;

            cpu.EU.Registers.BX = 0x10ff;
            cpu.EU.Registers.CL = 0xff;
            cpu.EU.Bus.SaveData8(cpu.EU.Registers.BX + 0x10, value8);
            cpu.NextInstruction();

            Assert.AreEqual(value8, cpu.EU.Registers.CL, "Instruction 0x8a failed");

        }

        [TestMethod]
        public void Test8b()
        {
            i8086CPU cpu = new i8086CPU();

            cpu.Boot(new byte[] { 0x8b, 0xc8 } /* MOV cx,ax (16 bit) */);

            cpu.EU.Registers.AX = 0x11ab;
            cpu.EU.Registers.CX = 0x22fe;
            cpu.NextInstruction();

            Assert.AreEqual(0x11ab, cpu.EU.Registers.CX, "Instruction 0x8b failed");
        }
    }
}
