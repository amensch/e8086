using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e8086Tests
{
    public class AddressModeTests
    {
        [Fact]
        public void TestAddressModeValues()
        {
            // 0110 1010
            var addr = new AddressMode(0x6a);

            Assert.Equal(0x01, addr.MOD);
            Assert.Equal(0x05, addr.REG);
            Assert.Equal(0x02, addr.RM);
        }
    }
}
