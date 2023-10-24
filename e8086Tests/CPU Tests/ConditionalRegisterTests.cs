using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e8086Tests
{
    public class ConditionalRegisterTests
    {
        #region Carry Flag
        [Fact]
        public void TestCarryFlagByteFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcCarryFlag(0, 0x8f);
            Assert.False(reg.CarryFlag);
        }
        [Fact]
        public void TestCarryFlagByteTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcCarryFlag(0, 0x48f);
            Assert.True(reg.CarryFlag);
        }
        [Fact]
        public void TestCarryFlagWordFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcCarryFlag(1, 0x8fa3);
            Assert.False(reg.CarryFlag);
        }
        [Fact]
        public void TestCarryFlagWordTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcCarryFlag(1, 0x5f48f);
            Assert.True(reg.CarryFlag);
        }
        #endregion

        #region Zero Flag
        [Fact]
        public void TestZeroFlagByteFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcZeroFlag(0, 0x8f);
            Assert.False(reg.ZeroFlag);
        }
        [Fact]
        public void TestZeroFlagByteTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcZeroFlag(0, 0);
            Assert.True(reg.ZeroFlag);
        }
        [Fact]
        public void TestZeroFlagWordFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcZeroFlag(1, 0x8fa3);
            Assert.False(reg.ZeroFlag);
        }
        [Fact]
        public void TestZeroFlagWordTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcZeroFlag(1, 0);
            Assert.True(reg.ZeroFlag);
        }
        #endregion

        #region Parity Flag
        [Fact]
        public void TestParityFlagTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcParityFlag(0x5478);
            Assert.True(reg.ParityFlag);
        }
        [Fact]
        public void TestParityFlagFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcParityFlag(0xa7);
            Assert.False(reg.ParityFlag);
        }
        #endregion

        #region Sign Flag
        [Fact]
        public void TestSignFlagByteFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcSignFlag(0, 0x2f);
            Assert.False(reg.SignFlag);
        }
        [Fact]
        public void TestSignFlagByteTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcSignFlag(0, 0xa9);
            Assert.True(reg.SignFlag);
        }
        [Fact]
        public void TestSignFlagWordFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcSignFlag(1, 0x58a9);
            Assert.False(reg.SignFlag);
        }
        [Fact]
        public void TestSignFlagWordTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcSignFlag(1, 0xf201);
            Assert.True(reg.SignFlag);
        }
        #endregion

        #region Aux Carry Flag
        [Fact]
        public void TestAuxCarryFlagFalse()
        {
            var reg = new ConditionalRegister();
            reg.CalcAuxCarryFlag(0x29, 0x42);
            Assert.False(reg.AuxCarryFlag);
        }
        [Fact]
        public void TestAuxCarryFlagTrue()
        {
            var reg = new ConditionalRegister();
            reg.CalcAuxCarryFlag(0x29, 0x4c);
            Assert.True(reg.AuxCarryFlag);
        }
        #endregion

        #region Overflow Flag

        [Fact]
        public void TestOverflowFlagByteFalse()
        {
            var reg = new ConditionalRegister();

            // normal case
            reg.CalcOverflowFlag(0, 0x32, 0x32);
            Assert.False(reg.OverflowFlag);

            // edge case (result=127)
            reg.CalcOverflowFlag(0, 0x64, 0x1b);
            Assert.False(reg.OverflowFlag);
        }
        [Fact]
        public void TestOverflowFlagByteTrue()
        {
            var reg = new ConditionalRegister();

            // normal case
            reg.CalcOverflowFlag(0, 0x64, 0x32);
            Assert.True(reg.OverflowFlag);

            // edge case (result=128)
            reg.CalcOverflowFlag(0, 0x64, 0x1c);
            Assert.True(reg.OverflowFlag);
        }

        [Fact]
        public void TestOverflowFlagWordFalse()
        {
            var reg = new ConditionalRegister();

            // normal case
            reg.CalcOverflowFlag(1, 0x1234, 0x0f89);
            Assert.False(reg.OverflowFlag);

            // edge case (result=0x7fff)
            reg.CalcOverflowFlag(1, 0x12d7, 0x6d28);
            Assert.False(reg.OverflowFlag);
        }
        [Fact]
        public void TestOverflowFlagWordTrue()
        {
            var reg = new ConditionalRegister();

            // normal case
            reg.CalcOverflowFlag(1, 0xf299, 0x8333);
            Assert.True(reg.OverflowFlag);

            // edge case (result=0x8000)
            reg.CalcOverflowFlag(1, 0x12d8, 0x6d28);
            Assert.True(reg.OverflowFlag);
        }
        #endregion


    }
}
