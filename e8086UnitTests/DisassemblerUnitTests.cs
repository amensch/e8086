using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDS.e8086;

namespace e8086UnitTests
{
    [TestClass]
    public class DisassemblerUnitTests
    {
        [TestMethod]
        public void Test00()
        {
            TestDasm(new byte[] { 0x00, 0xd8 }, "add al,bl" );
        }
        [TestMethod]
        public void Test01()
        {
            TestDasm(new byte[] { 0x01, 0xdb }, "add bx,bx");
            TestDasm(new byte[] { 0x01, 0xb1, 0x03, 0x73 }, "add [bx+di+7303],si");
        }
        [TestMethod]
        public void Test03()
        {
            TestDasm(new byte[] { 0x03, 0x1d }, "add bx,[di]");
        }
        [TestMethod]
        public void Test04()
        {
            TestDasm(new byte[] { 0x04, 0x0c }, "add al,0c");
        }
        [TestMethod]
        public void Test05()
        {
            TestDasm(new byte[] { 0x05, 0xbd, 0x7e }, "add ax,7ebd");
        }

        private string GetDasm(byte[] buffer)
        {
            string output = "";
            Disassemble8086.DisassembleNext(buffer, 0x00, 0, out output);
            return output;
        }

        private void TestDasm(byte[] buffer, string expected_result)
        {
            Assert.AreEqual(expected_result, GetDasm(buffer), expected_result + " failed");
        }
    }
}
