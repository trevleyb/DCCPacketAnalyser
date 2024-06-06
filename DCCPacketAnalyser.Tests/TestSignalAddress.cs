using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;

namespace DCCPacketAnalyser.Tests;

[TestFixture]
public class TestSignalAddress {
    [Test]
    public void TestThatWeGetTheCorrectSignalAddress() {
        //[TestCase(new byte[] {0x81,0x71,0x0A,0xFA}, 1, 10)]
        //[TestCase(new byte[] {0x83,0x73,0x06,0xF6}, 10, 6)]
        //[TestCase(new byte[] {0x83,0x77,0x01,0xF5}, 12, 1)]

        byte firstByte  = 0x81; // Starting data for Signal number 1
        byte secondByte = 0x71; // Sequence goes 71,73,75,77 then repeats
        var  address    = 1;
        for (var i = 0; i < 62; i++) {
            for (var j = 0; j < 4; j++) {
                var packetBytes = new byte[] { (byte)(firstByte + i), (byte)(secondByte + j * 2), 0x01, 0 };
                var checkSum    = (byte)packetBytes.Take(packetBytes.Length - 1).Aggregate(0, (current, t) => current ^ t);
                packetBytes[^1] = checkSum;
                var packet  = new PacketData(packetBytes);
                var message = PacketAnalyser.DeterminePacketType(packet);
                Assert.That(message, Is.Not.Null, "Packet Message should not be null unless an error occurred.");
                Assert.That(message, Is.InstanceOf<IPacketMessage>(), "Should not be possible as all results are IPacketMessage");
                Assert.That(message, Is.TypeOf<PacketMessage>(), "Return type, with this range should always be a Signal Message");
                Assert.That(message.Address, Is.EqualTo(address++), $"Address did not calculate correctly. Should be {address - 1}");
            }
        }
    }
}