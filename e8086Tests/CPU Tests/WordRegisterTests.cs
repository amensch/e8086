using KDS.e8086;

namespace e8086Tests
{
    public class WordRegisterTests
    {
        [Fact]
        public void TestValue()
        {
            // arrange
            var reg = new WordRegister();

            // act
            reg.Value = 0x85a7;

            // assert
            Assert.Equal(0x85, reg.HI);
            Assert.Equal(0xa7, reg.LO);
        }
    }
}
