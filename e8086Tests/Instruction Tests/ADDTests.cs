using KDS.e8086;
using KDS.e8086.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace e8086Tests
{
    public class ADDTests
    {
        /*
         * first byte is opcode-d-w
         * second byte is mod-reg-rm
         * third byte LO data/addr
         * fourth byte HI data/addr
         * fifth byte low data
         * sixth byte high data
         */

        private ExecutionUnit Setup(byte[] data)
        {
            var bus = new BusInterface(0, 0, data);
            var eu = new ExecutionUnit(bus);
            return eu;
        }

        [Fact]
        public void TestOpCode00_Reg_To_Reg()
        {
            // 00 d8  ADD AL,BL
            var bus = new BusInterface(0, 0, new byte[] { 0x00, 0xd8 });
            var eu = new ExecutionUnit(bus);

            // 95+67=fc;
            eu.Registers.AL = 0x95;
            eu.Registers.BL = 0x67;
            eu.Tick();

            Assert.Equal(0xfc, eu.Registers.AL);
            Assert.False(eu.CondReg.CarryFlag);
            Assert.False(eu.CondReg.ZeroFlag);
            Assert.True(eu.CondReg.ParityFlag);
            Assert.True(eu.CondReg.SignFlag);
            Assert.False(eu.CondReg.AuxCarryFlag);
            Assert.False(eu.CondReg.OverflowFlag);
        }

        [Fact]
        public void TestOpCode00_Mem_To_Reg()
        {
            // 00 36 15 01  ADD [0115],DH
            var bus = new BusInterface(0, 0, new byte[] { 0x00, 0x36, 0x15, 0x01 });
            var eu = new ExecutionUnit(bus);

            // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            eu.Bus.SaveData(0, 0x0115, 0x01); 
            eu.Registers.DH = 0xff;
            eu.Tick();

            Assert.Equal(0x00, eu.Bus.GetData(0, 0x0115));
            Assert.True(eu.CondReg.CarryFlag);
            Assert.True(eu.CondReg.ZeroFlag);
            Assert.True(eu.CondReg.ParityFlag);
            Assert.False(eu.CondReg.SignFlag);
            Assert.True(eu.CondReg.AuxCarryFlag);
            Assert.False(eu.CondReg.OverflowFlag);
        }

        [Fact]
        public void TestOpCode03_Reg_To_mem()
        {
            // 03 36 15 01  ADD SI,[0115]
            var bus = new BusInterface(0, 0, new byte[] { 0x03, 0x36, 0x15, 0x01 });
            var eu = new ExecutionUnit(bus);

            // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            eu.Bus.SaveData(1, 0x0115, 0x0100);
            eu.Registers.SI = 0xff00;
            eu.Tick();

            Assert.Equal(0x00, eu.Registers.SI);
            Assert.True(eu.CondReg.CarryFlag);
            Assert.True(eu.CondReg.ZeroFlag);
            Assert.True(eu.CondReg.ParityFlag);
            Assert.False(eu.CondReg.SignFlag);
            Assert.False(eu.CondReg.AuxCarryFlag);
            Assert.False(eu.CondReg.OverflowFlag);
        }

        [Fact]
        public void TestOpCode04_Imm_To_Reg()
        {
            // 04 40  ADD AL,40H
            var bus = new BusInterface(0, 0, new byte[] { 0x04, 0x40 });
            var eu = new ExecutionUnit(bus);

            // ff+01=100.  Carry=1, Parity=1, Zero=1, Sign=0, AuxCarry=0, Overflow=0
            eu.Bus.SaveData(1, 0x0115, 0x0100);
            eu.Registers.AL = 0x40;
            eu.Tick();

            Assert.Equal(0x80, eu.Registers.AL);
            Assert.False(eu.CondReg.CarryFlag);
            Assert.False(eu.CondReg.ZeroFlag);
            Assert.False(eu.CondReg.ParityFlag);
            Assert.True(eu.CondReg.SignFlag);
            Assert.False(eu.CondReg.AuxCarryFlag);
            Assert.True(eu.CondReg.OverflowFlag);
        }

    }
}
