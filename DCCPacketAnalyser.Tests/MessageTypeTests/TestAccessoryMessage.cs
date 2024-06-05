using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Tests.MessageTypeTests;

[TestFixture]
public class TestAccessoryMessage {
    [TestCase(new byte[] { 0x81, 0xF9, 0x78 }, typeof(AccessoryMessage), 1, AccessoryStateEnum.Normal)]
    [TestCase(new byte[] { 0x81, 0xF8, 0x79 }, typeof(AccessoryMessage), 1, AccessoryStateEnum.Reversed)]
    public void Test(byte[] packet, Type expectedType, int address, AccessoryStateEnum state) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.TypeOf(expectedType));
        Assert.That(decoded.AddressType, Is.EqualTo(AddressTypeEnum.Accessory));
        Assert.That(decoded.Address, Is.EqualTo(address));
        Assert.That(((AccessoryMessage)decoded).State, Is.EqualTo(state));
    }
}