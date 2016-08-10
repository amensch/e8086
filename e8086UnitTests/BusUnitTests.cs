using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class BusUnitTests
    {
        [TestMethod]
        public void TestInit()
        {
            i8086BusInterfaceUnit bus = new i8086BusInterfaceUnit(0, 0, new byte[] { 0x05, 0x10, 0x15 });

            int i = 0;
            Assert.AreEqual(0x05, bus.NextIP(), string.Format("NextIP failed {0}",i++));
            Assert.AreEqual(0x10, bus.NextIP(), string.Format("NextIP failed {0}", i++));
            Assert.AreEqual(0x15, bus.NextIP(), string.Format("NextIP failed {0}", i++));

            bus = new i8086BusInterfaceUnit(0x100, 0x200, new byte[] { 0x20, 0x25, 0x30 });
            Assert.AreEqual(0x20, bus.NextIP(), string.Format("NextIP failed {0}", i++));
            Assert.AreEqual(0x25, bus.NextIP(), string.Format("NextIP failed {0}", i++));
            Assert.AreEqual(0x30, bus.NextIP(), string.Format("NextIP failed {0}", i++));
        }

        [TestMethod]
        public void TestGetAndSave()
        {
            i8086BusInterfaceUnit bus = new i8086BusInterfaceUnit(0x100, 0x200, new byte[] { 0x05, 0x10, 0x15 });
            byte offset = 0x05;
            byte value8 = 0xaa;
            ushort value16 = 0x5f02;

            bus.DS = 0x300;
            bus.SaveData8(offset, value8);
            Assert.AreEqual(value8, bus.GetData8(offset), "GetData8 failed");

            bus.SaveData16(offset + 1, value16);
            Assert.AreEqual(value16, bus.GetData16(offset + 1), "GetData16 failed");

        }
    }
}
