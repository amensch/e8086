using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class EUTests_RotateAndShift
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
        public void Test_ROL()
        {
            CPU cpu = GetCPU(new byte[] { 0xd0, 0xc0 });/* ROL AL,1 */

            cpu.EU.Registers.AL = 0x1c;

            cpu.NextInstruction();
            Assert.AreEqual(0x38, cpu.EU.Registers.AL, "ROL result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ROL CF failed");
        }
        [TestMethod]
        public void Test_ROR()
        {
            CPU cpu = GetCPU(new byte[] { 0xd0, 0xc8 });/* ROR AL,1 */

            cpu.EU.Registers.AL = 0x1c;

            cpu.NextInstruction();
            Assert.AreEqual(0x0e, cpu.EU.Registers.AL, "ROR result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "ROR CF failed");
        }
        [TestMethod]
        public void Test_RCL()
        {
            CPU cpu = GetCPU(new byte[] { 0xd0, 0xd0 });/* RCL AL,1 */

            cpu.EU.CondReg.CarryFlag = true;
            cpu.EU.Registers.AL = 0x1c;

            cpu.NextInstruction();
            Assert.AreEqual(0x39, cpu.EU.Registers.AL, "RCL result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "RCL CF failed");
        }
        [TestMethod]
        public void Test_RCR()
        {
            CPU cpu = GetCPU(new byte[] { 0xd0, 0xd8 });/* RCR AL,1 */

            cpu.EU.CondReg.CarryFlag = true;
            cpu.EU.Registers.AL = 0x1c;

            cpu.NextInstruction();
            Assert.AreEqual(0x8e, cpu.EU.Registers.AL, "RCR result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "RCR CF failed");
        }
        [TestMethod]
        public void Test_SAL()
        {
            CPU cpu = GetCPU(new byte[] { 0xd0, 0xe0 });/* SAL AL,1 */

            cpu.EU.Registers.AL = 0xe0;

            cpu.NextInstruction();
            Assert.AreEqual(0xc0, cpu.EU.Registers.AL, "SAL result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SAL CF failed");
        }
        [TestMethod]
        public void Test_SHR()
        {
            CPU cpu = GetCPU(new byte[] { 0xd0, 0xe8 });/* SHR AL,1 */

            cpu.EU.Registers.AL = 0x07;

            cpu.NextInstruction();
            Assert.AreEqual(0x03, cpu.EU.Registers.AL, "SHR result failed");
            Assert.AreEqual(true, cpu.EU.CondReg.CarryFlag, "SHR CF failed");
        }
        [TestMethod]
        public void Test_SAR()
        {
            CPU cpu = GetCPU(new byte[] { 0xd0, 0xf8, /* SAR AL,1 */
                                                0xd2, 0xfb }); /* SAR BL, CL */

            cpu.EU.Registers.AL = 0xe0;
            cpu.EU.Registers.BL = 0x4c;
            cpu.EU.Registers.CL = 0x01;

            cpu.NextInstruction();
            Assert.AreEqual(0xf0, cpu.EU.Registers.AL, "SAR 1 result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SAR 1 CF failed");

            cpu.NextInstruction();
            Assert.AreEqual(0x26, cpu.EU.Registers.BL, "SAR 2 result failed");
            Assert.AreEqual(false, cpu.EU.CondReg.CarryFlag, "SAR 2 CF failed");
        }
    }
}
