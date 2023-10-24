using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e8086Tests
{
    public class BusTests
    {
        private byte[] data =
        {
            0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde, 0xf0,
            0x00, 0x91, 0x82, 0x73, 0x64, 0x53, 0x44, 0x88
        };

        [Fact]
        public void TestLoadROM()
        {
            var bus = new BusInterface();
            bus.LoadROM(data, 0);
            Assert.NotNull(bus);
            Assert.Equal(0x9a, bus.GetData(0, 4));
            Assert.Equal(0xf0de, bus.GetData(1, 6));
        }

        [Fact]
        public void TestLoadBIOS()
        {
            int offset = BusInterface.MAX_MEMORY - data.GetLength(0);
            var bus = new BusInterface();
            bus.LoadBIOS(data);
            Assert.NotNull(bus);
            Assert.Equal(0x9a, bus.GetData(0, offset + 4));
            Assert.Equal(0xf0de, bus.GetData(1, offset + 6));
        }

        [Fact]
        public void TestNextImmediate()
        {
            var bus = new BusInterface(0, 0, data);
            Assert.Equal(0x12, bus.NextImmediate());
            Assert.Equal(0x34, bus.NextImmediate());
            Assert.Equal(0x56, bus.NextImmediate());
        }

        [Fact]
        public void TestGetDataByte()
        {
            var bus = new BusInterface(0, 0, data);
            Assert.Equal(0x9a, bus.GetData(0, 4));
        }

        [Fact]
        public void TestGetDataWord()
        {
            var bus = new BusInterface(0, 0, data);
            Assert.Equal(0xf0de, bus.GetData(1, 6));
        }

        [Fact]
        public void TestSaveDataByte()
        {
            var bus = new BusInterface(0, 0, data);
            bus.SaveData(0, 4, 0x11);
            Assert.Equal(0x11, bus.GetData(0, 4));
        }

        [Fact]
        public void TestSaveDataWord()
        {
            var bus = new BusInterface(0, 0, data);
            bus.SaveData(1, 6, 0x7722);
            Assert.Equal(0x7722, bus.GetData(1, 6));
        }

        [Fact]
        public void TestGetWord()
        {
            var bus = new BusInterface(0, 0, data);
            Assert.Equal(0xf0de, bus.GetWord(0, 6));
        }

        [Fact]
        public void TestMoveStringByte()
        {
            var bus = new BusInterface(0, 0, data);
            bus.MoveString(0, 0, 8);
            Assert.Equal(0x12, bus.GetData(0, 8));
        }
        
        [Fact]
        public void TestMoveStringWord()
        {
            var bus = new BusInterface(0, 0, data);
            bus.MoveString(1, 0, 8);
            Assert.Equal(0x3412, bus.GetData(1, 8));
        }
        [Fact]
        public void TestGetDestStringByte()
        {
            var bus = new BusInterface(0, 0, data);
            Assert.Equal(0x9a, bus.GetDestString(0, 4));
        }
        [Fact]
        public void TestGetDestStringWord()
        {
            var bus = new BusInterface(0, 0, data);
            Assert.Equal(0xbc9a, bus.GetDestString(1, 4));
        }

        [Fact]
        public void SaveStringByte()
        {
            var bus = new BusInterface(0, 0, data);
            bus.SaveString(0, 7, 0xdd);
            Assert.Equal(0xdd, bus.GetDestString(0, 7));
        }

        [Fact]
        public void SaveStringWord()
        {
            var bus = new BusInterface(0, 0, data);
            bus.SaveString(1, 7, 0xddcc);
            Assert.Equal(0xddcc, bus.GetDestString(1, 7));
        }

        [Fact]
        public void TestStack()
        {
            var bus = new BusInterface(0, 0, data);
            int SP = 8;

            SP -= 2;
            bus.PushStack(SP, 0xaabb);

            SP -= 2;
            bus.PushStack(SP, 0xccdd);

            var result = bus.PopStack(SP);
            SP += 2;
            Assert.Equal(0xccdd, result);

            result = bus.PopStack(SP);
            SP += 2;
            Assert.Equal(0xaabb, result);
        }
    }
}
