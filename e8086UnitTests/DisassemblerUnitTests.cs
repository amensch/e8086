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
        public void Test02()
        {
            TestDasm(new byte[] { 0x02, 0x00 }, "add al,[bx+si]", 2);
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

        [TestMethod]
        public void Test08()
        {
            TestDasm(new byte[] { 0x08, 0xd8 }, "or al,bl", 2);
        }
        [TestMethod]
        public void Test09()
        {
            TestDasm(new byte[] { 0x09, 0xdb }, "or bx,bx", 2);
            TestDasm(new byte[] { 0x09, 0xb1, 0x03, 0x73 }, "or [bx+di+7303],si", 4);
            TestDasm(new byte[] { 0x09, 0x2e, 0x80, 0x0c }, "or [0c80],bp", 4);
        }
        [TestMethod]
        public void Test0A()
        {
            TestDasm(new byte[] { 0x0a, 0x00 }, "or al,[bx+si]", 2);
        }
        [TestMethod]
        public void Test0B()
        {
            TestDasm(new byte[] { 0x0b, 0x1d }, "or bx,[di]", 2);
        }
        [TestMethod]
        public void Test0C()
        {
            TestDasm(new byte[] { 0x0c, 0x0c }, "or al,0c", 2);
        }
        [TestMethod]
        public void Test0D()
        {
            TestDasm(new byte[] { 0x0d, 0xbd, 0x7e }, "or ax,7ebd", 3);
        }
        [TestMethod]
        public void Test72()
        {
            TestDasm(new byte[] { 0x72, 0x20, 0x90 }, "jb 22", 2);
        }
        [TestMethod]
        public void Test80()
        {
            TestDasm(new byte[] { 0x80, 0xbc, 0x7f, 0x75, 0x2b }, "cmp [si+757f],2b", 5);
            TestDasm(new byte[] { 0x80, 0xab, 0xd6, 0x47, 0x1e }, "sub [bp+di+47d6],1e", 5);
        }
        [TestMethod]
        public void Test81()
        {
            TestDasm(new byte[] { 0x81, 0x33, 0x6d, 0x20 }, "xor [bp+di],206d", 4);
        }
        [TestMethod]
        public void Test82()
        {
            TestDasm(new byte[] { 0x82, 0x55, 0xfd, 0x22 }, "adc [di+fd],22", 4);
        }
        [TestMethod]
        public void Test86()
        {
            TestDasm(new byte[] { 0x86, 0x03 }, "xchg al,[bp+di]", 2);
            TestDasm(new byte[] { 0x86, 0xd6 }, "xchg dl,dh", 2);
        }
        [TestMethod]
        public void Test87()
        {
            TestDasm(new byte[] { 0x87, 0x04 }, "xchg ax,[si]", 2);
        }
        [TestMethod]
        public void Test88()
        {
            TestDasm(new byte[] { 0x88, 0x05 }, "mov [di],al", 2);
        }
        [TestMethod]
        public void Test8A()
        {
            TestDasm(new byte[] { 0x8a, 0x88, 0xf6, 0x12 }, "mov cl,[bx+si+12f6]", 4);
        }
        [TestMethod]
        public void Test8C()
        {
            TestDasm(new byte[] { 0x8c, 0xc8 }, "mov ax,cs", 2);
        }
        [TestMethod]
        public void Test8D()
        {
            TestDasm(new byte[] { 0x8d, 0x0c }, "lea cx,[si]", 2);
        }
        [TestMethod]
        public void Test8E()
        {
            TestDasm(new byte[] { 0x8e, 0xd8 }, "mov ds,ax", 2);
        }
        [TestMethod]
        public void Test8F()
        {
            TestDasm(new byte[] { 0x8f, 0x05 }, "pop [di]", 2);
            TestDasm(new byte[] { 0x8f, 0x06, 0xcb, 0x03 }, "pop [03cb]", 4);
        }
        [TestMethod]
        public void Test9A()
        {
            TestDasm(new byte[] { 0x9a, 0x56, 0xb7, 0x24, 0xdc }, "call dc24:b756", 5);
        }
        [TestMethod]
        public void TestA0()
        {
            TestDasm(new byte[] { 0xa0, 0x0f, 0xa8 }, "mov al,[a80f]", 3);
        }
        [TestMethod]
        public void TestA1()
        {
            TestDasm(new byte[] { 0xa1, 0xdf, 0xa3 }, "mov ax,[a3df]", 3);
        }
        [TestMethod]
        public void TestA2()
        {
            TestDasm(new byte[] { 0xa2, 0xcc, 0x0b }, "mov [0bcc],al", 3);
        }
        [TestMethod]
        public void TestB5()
        {
            TestDasm(new byte[] { 0xb5, 0xdd }, "mov ch,dd", 2);
        }
        [TestMethod]
        public void TestC2()
        {
            TestDasm(new byte[] { 0xc2, 0xb7, 0xe6 }, "ret e6b7", 3);
        }
        [TestMethod]
        public void TestC4()
        {
            TestDasm(new byte[] { 0xc4, 0x18 }, "les bx,[bx+si]", 2);
        }
        [TestMethod]
        public void TestC5()
        {
            TestDasm(new byte[] { 0xc5, 0x66, 0x43}, "lds sp,[bp+43]", 3);
            TestDasm(new byte[] { 0xc5, 0x02 }, "lds ax,[bp+si]", 2);
        }
        [TestMethod]
        public void TestC6()
        {
            TestDasm(new byte[] { 0xc6, 0x05, 0x99 }, "mov [di],99", 3);
        }
        [TestMethod]
        public void TestCA()
        {
            TestDasm(new byte[] { 0xca, 0x7b, 0xb6 }, "retf b67b", 3);
        }
        [TestMethod]
        public void TestCD()
        {
            TestDasm(new byte[] { 0xcd, 0x1b }, "int 1b", 2);
        }
        [TestMethod]
        public void TestD0()
        {
            TestDasm(new byte[] { 0xd0, 0x0f }, "ror [bx],1", 2);
        }
        [TestMethod]
        public void TestD2()
        {
            TestDasm(new byte[] { 0xd2, 0x15 }, "rcl [di],cl", 2);
        }
        [TestMethod]
        public void TestD5()
        {
            TestDasm(new byte[] { 0xd5, 0x62 }, "aad 62", 2);
        }
        [TestMethod]
        public void TestE7()
        {
            TestDasm(new byte[] { 0xe7, 0x05 }, "out 05,ax", 2);
        }
        [TestMethod]
        public void TestE9()
        {
            TestDasm(new byte[] { 0xe9, 0x09, 0x00 }, "jmp 000c", 3);
        }
        [TestMethod]
        public void TestF6()
        {
            TestDasm(new byte[] { 0xf6, 0xfe }, "idiv dh", 2);
        }
        [TestMethod]
        public void TestFF()
        {
            TestDasm(new byte[] { 0xff, 0xf0 }, "push ax", 2);
        }
    }
}
