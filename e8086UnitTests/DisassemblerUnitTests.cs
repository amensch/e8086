using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class DisassemblerUnitTests
    {

        private void TestDasm(byte[] buffer, string expected_result, int expected_bytes_read)
        {
            string output = "";
            UInt32 bytes_read;

            bytes_read = Disassemble8086.DisassembleNext(buffer, 0x00, 0, out output);

            Assert.AreEqual(expected_result, output, expected_result + " failed");
            Assert.AreEqual(expected_bytes_read, (int)bytes_read, expected_result + " size failed");
        }

        [TestMethod]
        public void Test00()
        {
            TestDasm(new byte[] { 0x00, 0xd8 }, "add al,bl", 2 );
        }
        [TestMethod]
        public void Test01()
        {
            TestDasm(new byte[] { 0x01, 0xdb }, "add bx,bx", 2);
            TestDasm(new byte[] { 0x01, 0xb1, 0x03, 0x73 }, "add [bx+di+7303],si", 4);
        }
        [TestMethod]
        public void Test03()
        {
            TestDasm(new byte[] { 0x03, 0x1d }, "add bx,[di]", 2);
        }
        [TestMethod]
        public void Test04()
        {
            TestDasm(new byte[] { 0x04, 0x0c }, "add al,0c", 2);
        }
        [TestMethod]
        public void Test05()
        {
            TestDasm(new byte[] { 0x05, 0xbd, 0x7e }, "add ax,7ebd", 3);
        }

    }
}
