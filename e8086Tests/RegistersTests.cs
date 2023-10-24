using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e8086Tests
{
    public class RegistersTests
    {
        [Fact]
        public void TestAL()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_AL, 0x05);
            Assert.Equal(0x05, reg.AL);
            Assert.Equal(0x00, reg.AH);
            Assert.Equal(0x05, reg.AX);
            Assert.Equal(0x05, reg.GetRegisterValue(0, Registers.REGISTER_AL));
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_AH));
            Assert.Equal(0x05, reg.GetRegisterValue(1, Registers.REGISTER_AX));
        }
        [Fact]
        public void TestCL()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_CL, 0x05);
            Assert.Equal(0x05, reg.CL);
            Assert.Equal(0x00, reg.CH);
            Assert.Equal(0x05, reg.CX);
            Assert.Equal(0x05, reg.GetRegisterValue(0, Registers.REGISTER_CL));
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_CH));
            Assert.Equal(0x05, reg.GetRegisterValue(1, Registers.REGISTER_CX));
        }
        [Fact]
        public void TestDL()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_DL, 0x05);
            Assert.Equal(0x05, reg.DL);
            Assert.Equal(0x00, reg.DH);
            Assert.Equal(0x05, reg.DX);
            Assert.Equal(0x05, reg.GetRegisterValue(0, Registers.REGISTER_DL));
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_DH));
            Assert.Equal(0x05, reg.GetRegisterValue(1, Registers.REGISTER_DX));
        }
        [Fact]
        public void TestBL()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_BL, 0x05);
            Assert.Equal(0x05, reg.BL);
            Assert.Equal(0x00, reg.BH);
            Assert.Equal(0x05, reg.BX);
            Assert.Equal(0x05, reg.GetRegisterValue(0, Registers.REGISTER_BL));
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_BH));
            Assert.Equal(0x05, reg.GetRegisterValue(1, Registers.REGISTER_BX));
        }
        [Fact]
        public void TestAH()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_AH, 0x7a);
            Assert.Equal(0x00, reg.AL);
            Assert.Equal(0x7a, reg.AH);
            Assert.Equal(0x7a00, reg.AX);
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_AL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_AH));
            Assert.Equal(0x7a00, reg.GetRegisterValue(1, Registers.REGISTER_AX));
        }
        [Fact]
        public void TestCH()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_CH, 0x7a);
            Assert.Equal(0x00, reg.CL);
            Assert.Equal(0x7a, reg.CH);
            Assert.Equal(0x7a00, reg.CX);
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_CL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_CH));
            Assert.Equal(0x7a00, reg.GetRegisterValue(1, Registers.REGISTER_CX));
        }
        [Fact]
        public void TestDH()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_DH, 0x7a);
            Assert.Equal(0x00, reg.DL);
            Assert.Equal(0x7a, reg.DH);
            Assert.Equal(0x7a00, reg.DX);
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_DL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_DH));
            Assert.Equal(0x7a00, reg.GetRegisterValue(1, Registers.REGISTER_DX));
        }
        [Fact]
        public void TestBH()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(0, Registers.REGISTER_BH, 0x7a);
            Assert.Equal(0x00, reg.BL);
            Assert.Equal(0x7a, reg.BH);
            Assert.Equal(0x7a00, reg.BX);
            Assert.Equal(0x00, reg.GetRegisterValue(0, Registers.REGISTER_BL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_BH));
            Assert.Equal(0x7a00, reg.GetRegisterValue(1, Registers.REGISTER_BX));
        }
        [Fact]
        public void TestAX()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_AX, 0x7a19);
            Assert.Equal(0x19, reg.AL);
            Assert.Equal(0x7a, reg.AH);
            Assert.Equal(0x7a19, reg.AX);
            Assert.Equal(0x19, reg.GetRegisterValue(0, Registers.REGISTER_AL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_AH));
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_AX));
        }
        [Fact]
        public void TestCX()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_CX, 0x7a19);
            Assert.Equal(0x19, reg.CL);
            Assert.Equal(0x7a, reg.CH);
            Assert.Equal(0x7a19, reg.CX);
            Assert.Equal(0x19, reg.GetRegisterValue(0, Registers.REGISTER_CL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_CH));
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_CX));
        }
        [Fact]
        public void TestDX()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_DX, 0x7a19);
            Assert.Equal(0x19, reg.DL);
            Assert.Equal(0x7a, reg.DH);
            Assert.Equal(0x7a19, reg.DX);
            Assert.Equal(0x19, reg.GetRegisterValue(0, Registers.REGISTER_DL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_DH));
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_DX));
        }
        [Fact]
        public void TestBX()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_BX, 0x7a19);
            Assert.Equal(0x19, reg.BL);
            Assert.Equal(0x7a, reg.BH);
            Assert.Equal(0x7a19, reg.BX);
            Assert.Equal(0x19, reg.GetRegisterValue(0, Registers.REGISTER_BL));
            Assert.Equal(0x7a, reg.GetRegisterValue(0, Registers.REGISTER_BH));
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_BX));
        }
        [Fact]
        public void TestSP()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_SP, 0x7a19);
            Assert.Equal(0x7a19, reg.SP);
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_SP));
        }
        [Fact]
        public void TestBP()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_BP, 0x7a19);
            Assert.Equal(0x7a19, reg.BP);
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_BP));
        }
        [Fact]
        public void TestSI()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_SI, 0x7a19);
            Assert.Equal(0x7a19, reg.SI);
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_SI));
        }
        [Fact]
        public void TestDI()
        {
            var reg = new Registers();
            reg.SaveRegisterValue(1, Registers.REGISTER_DI, 0x7a19);
            Assert.Equal(0x7a19, reg.DI);
            Assert.Equal(0x7a19, reg.GetRegisterValue(1, Registers.REGISTER_DI));
        }
    }
}
