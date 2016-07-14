using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    /// <summary>
    /// Summary description for EUTests_StringTests
    /// </summary>
    [TestClass]
    public class EUTests_StringTests
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
        public void Test_A4_MOVS8()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xa4, 0xa4, 0xa4, 0xa4, 0xa4 });

            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseDS;
            cpu.EU.Registers.SI = 0x100;
            cpu.EU.Registers.DI = 0x200;

            // write a bunch of source data to DS:offset
            byte value = 0xa1;
            for (int ii = cpu.EU.Registers.SI; ii <= 0x120; ii++ )
            {
                cpu.EU.Bus.SaveData8(ii, value++);
            }

            // write a bunch of data to ES:offset (to show the correct data is overwritten)
            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseES;
            value = 0x55;
            for (int ii = cpu.EU.Registers.DI; ii <= 0x220; ii++)
            {
                cpu.EU.Bus.SaveData8(ii, value++);
            }

            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.NoOverride;
            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();

            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseES;
            Assert.AreEqual(0xa1, cpu.EU.Bus.GetData8(0x200), "MOVS 1 result failed");
            Assert.AreEqual(0xa2, cpu.EU.Bus.GetData8(0x201), "MOVS 2 result failed");
            Assert.AreEqual(0xa3, cpu.EU.Bus.GetData8(0x202), "MOVS 3 result failed");
            Assert.AreEqual(0xa4, cpu.EU.Bus.GetData8(0x203), "MOVS 4 result failed");
            Assert.AreEqual(0xa5, cpu.EU.Bus.GetData8(0x204), "MOVS 5 result failed");
        }


        [TestMethod]
        public void Test_A5_MOVS16()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xa5, 0xa5, 0xa5, 0xa5, 0xa5 });

            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseDS;

            cpu.EU.Registers.SI = 0x100;
            cpu.EU.Registers.DI = 0x200;

            // write a bunch of source data to DS:offset
            byte value = 0xa1;
            for (int ii = cpu.EU.Registers.SI; ii <= 0x120; ii++)
            {
                cpu.EU.Bus.SaveData8(ii, value++);
            }

            // write a bunch of data to ES:offset (to show the correct data is overwritten)
            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseES;
            value = 0x55;
            for (int ii = cpu.EU.Registers.DI; ii <= 0x220; ii++)
            {
                cpu.EU.Bus.SaveData8(ii, value++);
            }

            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.NoOverride;
            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();
            cpu.NextInstruction();

            cpu.EU.Bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseES;
            Assert.AreEqual(0xa2a1, cpu.EU.Bus.GetData16(0x200), "MOVSW 1 result failed");
            Assert.AreEqual(0xa4a3, cpu.EU.Bus.GetData16(0x202), "MOVSW 2 result failed");
            Assert.AreEqual(0xa6a5, cpu.EU.Bus.GetData16(0x204), "MOVSW 3 result failed");
            Assert.AreEqual(0xa8a7, cpu.EU.Bus.GetData16(0x206), "MOVSW 4 result failed");
            Assert.AreEqual(0xaaa9, cpu.EU.Bus.GetData16(0x208), "MOVSW 5 result failed");
        }

        [TestMethod]
        public void Test_LODS()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xac, 0xad });

            cpu.EU.Registers.SI = 0x200;
            cpu.EU.Registers.AX = 0x0e00;
            cpu.EU.Bus.SaveData8(cpu.EU.Registers.SI, 0xc7);

            cpu.NextInstruction();
            Assert.AreEqual(0xc7, cpu.EU.Registers.AL, "LODSB 1 result failed");
            Assert.AreEqual(0x201, cpu.EU.Registers.SI, "LODSB 2 result failed");

            cpu.EU.Registers.SI = 0x300;
            cpu.EU.Registers.AX = 0xffff;
            cpu.EU.Bus.SaveData16(cpu.EU.Registers.SI, 0xa965);

            cpu.NextInstruction();
            Assert.AreEqual(0xa965, cpu.EU.Registers.AX, "LODSW 2 result failed");
            Assert.AreEqual(0x302, cpu.EU.Registers.SI, "LODSW 2 result failed");
              
        }

        [TestMethod]
        public void Test_SODS()
        {
            i8086CPU cpu = GetCPU(new byte[] { 0xaa, 0xab });

            cpu.EU.Registers.DI = 0x200;
            cpu.EU.Registers.AL = 0xf8;

            cpu.NextInstruction();
            Assert.AreEqual(0xf8, cpu.EU.Bus.GetDestString8(0x200), "STOSB result failed");
            Assert.AreEqual(0x201, cpu.EU.Registers.DI, "STOSB DI failed");

            cpu.EU.Registers.DI = 0x300;
            cpu.EU.Registers.AX = 0x12fe;

            cpu.NextInstruction();
            Assert.AreEqual(0x12fe, cpu.EU.Bus.GetDestString16(0x300), "STOSW result failed");
            Assert.AreEqual(0x302, cpu.EU.Registers.DI, "STOSW DI failed");

        }
    }
}
