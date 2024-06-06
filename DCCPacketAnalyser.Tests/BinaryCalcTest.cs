
namespace DCCPacketAnalyser.Tests;

[TestFixture]
public class BinaryCalcTest {
    [Test]
    public void TestInALoop() {
        // First Byte = xxAAAAAA
        // Second Byte = 0AAA0aa1
        // First part, we want the AAAAAA from first byte as the lower of the full address with AAA from second
        // byte the second AAA is a 1s compliment
        // Second part, subtract 1, shift 1 byte  and add the aa to it to make an 11 bit address

        byte expected = 1;
        for (byte firstByte = 1; firstByte < 0b00111111; firstByte++) {
            for (byte thirdByte = 0; thirdByte < 4; thirdByte++) {
                var secondByte = (byte)(0b01110001 | (thirdByte << 1));
                Console.Write($"Testing: {firstByte:b8} | {secondByte:b8} = ");
                var result = CombineBits(firstByte, secondByte);
                Console.WriteLine($"{expected:D3} = {result:D3}:{result:b8}");
                Assert.That(result, Is.EqualTo(expected));
                expected++;
            }
        }
    }

    private static int CombineBits(byte address, byte instByte) {
        // This works, but I don't think it is the right solution
        address  &= 0b00111111;
        instByte &= 0b00000110;
        instByte =  (byte)(instByte >> 1);
        var combinedAddress = (((address - 1) << 2) | instByte) + 1;
        return combinedAddress;
    }
}