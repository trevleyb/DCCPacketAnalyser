using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Tests.MessageTypeTests;

[TestFixture]
public class TestFunctionsMessage {
    [TestCase(new byte[] { 0xC4, 0xD2, 0x80, 0x96 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F0F4, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xB0, 0xA6 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F5F8, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0x80, 0x96 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F0F4, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xB0, 0xA6 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F5F8, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0x80, 0x96 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F0F4, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xB0, 0xA6 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F5F8, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0x80, 0x96 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F0F4, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xB0, 0xA6 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F5F8, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0x90, 0x86 }, typeof(FunctionsMessage), 1234, FunctionsGroupEnum.F0F4, 0)]
    public void Test(byte[] packet, Type expectedType, int address, FunctionsGroupEnum group, byte bitValues) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.TypeOf(expectedType));
        Assert.That(decoded.AddressType, Is.AnyOf(AddressTypeEnum.Long, AddressTypeEnum.Short, AddressTypeEnum.Broadcast));
        Assert.That(decoded.Address, Is.EqualTo(address));
        Assert.That(((FunctionsMessage)decoded).Group, Is.EqualTo(group));
        Assert.That(((FunctionsMessage)decoded).BitValues, Is.EqualTo(bitValues));
    }
}