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
        public void TestNextImmediate()
        {
            var bus = new BusInterface(0, 0, data);

            var next = bus.NextImmediate();
            Assert.Equal(0x12, next);

            next = bus.NextImmediate();
            Assert.Equal(0x34, next);

            next = bus.NextImmediate();
            Assert.Equal(0x56, next);
        }

        [Fact]
        public void TestGetDataByte()
        {
            var bus = new BusInterface(0, 0, data);

            var result = bus.GetData(0, 4);
            Assert.Equal(0x9a, result);
        }

        [Fact]
        public void TestGetDataWord()
        {
            var bus = new BusInterface(0, 0, data);

            var result = bus.GetData(1, 6);
            Assert.Equal(0xf0de, result);
        }

        [Fact]
        public void TestSaveDataByte()
        {
            var bus = new BusInterface(0, 0, data);

            bus.SaveData(0, 4, 0x11);
            var result = bus.GetData(0, 4);
            Assert.Equal(0x11, result);
        }

        [Fact]
        public void TestSaveDataWord()
        {
            var bus = new BusInterface(0, 0, data);

            bus.SaveData(1, 6, 0x7722);
            var result = bus.GetData(1, 6);
            Assert.Equal(0x7722, result);
        }

        [Fact]
        public void TestGetWord()
        {
            var bus = new BusInterface(0, 0, data);
            var result = bus.GetWord(0, 6);
            Assert.Equal(0xf0de, result);
        }

        [Fact]
        public void TestMoveStringByte()
        {
            var bus = new BusInterface(0, 0, data);
            bus.MoveString(0, 0, 8);
            var result = bus.GetData(0, 8);
            Assert.Equal(0x12, result);
        }

        public void TestMoveStringWord()
        {

        }

        public void TestGetDestStringByte()
        {

        }

        public void TestGetDestStringWord()
        {

        }

        public void SaveStringByte()
        {

        }

        public void SaveStringWord()
        {

        }

        public void TestPopStack()
        {

        }

        public void TestPushStack()
        {

        }


    }
}
