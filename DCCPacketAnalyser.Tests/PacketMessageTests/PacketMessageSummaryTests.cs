using DCCPacketAnalyser.Analyser.Base;

namespace DCCPacketAnalyser.Tests.PacketMessageTests;

[TestFixture]
public class PacketMessageSummaryTests {
    [TestCase(AddressTypeEnum.Short, "S0001")]
    [TestCase(AddressTypeEnum.Long, "L0001")]
    [TestCase(AddressTypeEnum.Broadcast, "B0001")]
    [TestCase(AddressTypeEnum.Accessory, "A0001")]
    [TestCase(AddressTypeEnum.Signal, "G0001")]
    [TestCase(AddressTypeEnum.Idle, "X0001")]
    [TestCase(AddressTypeEnum.Error, "X0001")]
    [TestCase(AddressTypeEnum.Duplicate, "X0001")]
    public void AddressAsString_ReturnsExpectedString_GivenAddressTypeEnum(AddressTypeEnum addressType, string expected) {
        var packetMessage = new PacketMessage(new PacketData(new byte[] { 0x01 }), addressType, 1);
        var result        = packetMessage.AddressAsString;
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Detailed_ReturnsExpectedString_WhenCalled() {
        var packetMessage = new PacketMessage(new PacketData(new byte[] { 0x01 }), AddressTypeEnum.Short, 1);
        var result        = packetMessage.Detailed;
        Assert.That(result, Is.EqualTo("S0001"));
    }

    [Test]
    public void Summary_ReturnsExpectedString_WhenCalled() {
        var packetMessage = new PacketMessage(new PacketData(new byte[] { 0x01 }), AddressTypeEnum.Short, 1);
        var result        = packetMessage.Summary;
        Assert.That(result, Is.EqualTo("S0001"));
    }
}