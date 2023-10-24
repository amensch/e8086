using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e8086Tests
{
    public class RAMTests
    {
        private const int RAM_SIZE = 0x10;
        private byte[] data =
        {
            0x12,
            0x34,
            0x56,
            0x78,
            0x9a,
            0xbc,
            0xde,
            0xf0,
            0x00,
            0x91,
            0x82,
            0x73,
            0x64,
            0x53,
            0x44,
            0x88
        };


        [Fact]
        public void TestLoad()
        {
            var ram = new RAM(RAM_SIZE);
            ram.Load(data, 0);

            Assert.Equal(0x9a, ram[4]);
            Assert.Equal(new byte[] { 0xde, 0xf0, 0x00 }, ram.GetChunk(6, 3));
        }
    }
}
