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
            BusInterface bus = new BusInterface(0, 0, new byte[] { 0x05, 0x10, 0x15 });

            int i = 0;
            Assert.AreEqual(0x05, bus.NextImmediate(), string.Format("NextIP failed {0}",i++));
            Assert.AreEqual(0x10, bus.NextImmediate(), string.Format("NextIP failed {0}", i++));
            Assert.AreEqual(0x15, bus.NextImmediate(), string.Format("NextIP failed {0}", i++));

            bus = new BusInterface(0x100, 0x200, new byte[] { 0x20, 0x25, 0x30 });
            Assert.AreEqual(0x20, bus.NextImmediate(), string.Format("NextIP failed {0}", i++));
            Assert.AreEqual(0x25, bus.NextImmediate(), string.Format("NextIP failed {0}", i++));
            Assert.AreEqual(0x30, bus.NextImmediate(), string.Format("NextIP failed {0}", i++));
        }

        [TestMethod]
        public void TestGetAndSave()
        {
            BusInterface bus = new BusInterface(0x100, 0x200, new byte[] { 0x05, 0x10, 0x15 });
            byte offset = 0x05;
            byte value8 = 0xaa;
            ushort value16 = 0x5f02;

            bus.DS = 0x300;
            bus.SaveByte(offset, value8);
            Assert.AreEqual(value8, bus.GetData(0, offset), "GetData8 failed");

            bus.SaveWord(offset + 1, value16);
            Assert.AreEqual(value16, bus.GetData(1, offset + 1), "GetData16 failed");

        }
    }
}
