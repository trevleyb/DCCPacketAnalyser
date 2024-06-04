using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Tests.MessageTypeTests;

[TestFixture]
public class TestConfigCVMessage {

    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x30, 0xC8 }, typeof(ConfigCVMessage), 1234, 3, 48)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x30, 0xC9 }, typeof(ConfigCVMessage), 1234, 4, 48)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x00, 0xF8 }, typeof(ConfigCVMessage), 1234, 3, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x00, 0xF9 }, typeof(ConfigCVMessage), 1234, 4, 0)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x08, 0xF0 }, typeof(ConfigCVMessage), 1234, 3, 8)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x08, 0xF1 }, typeof(ConfigCVMessage), 1234, 4, 8)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x10, 0xE8 }, typeof(ConfigCVMessage), 1234, 3, 16)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x10, 0xE9 }, typeof(ConfigCVMessage), 1234, 4, 16)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x18, 0xE0 }, typeof(ConfigCVMessage), 1234, 3, 24)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x18, 0xE1 }, typeof(ConfigCVMessage), 1234, 4, 24)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x20, 0xD8 }, typeof(ConfigCVMessage), 1234, 3, 32)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x20, 0xD9 }, typeof(ConfigCVMessage), 1234, 4, 32)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x28, 0xD0 }, typeof(ConfigCVMessage), 1234, 3, 40)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x28, 0xD1 }, typeof(ConfigCVMessage), 1234, 4, 40)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x38, 0xC0 }, typeof(ConfigCVMessage), 1234, 3, 56)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x38, 0xC1 }, typeof(ConfigCVMessage), 1234, 4, 56)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x40, 0xB8 }, typeof(ConfigCVMessage), 1234, 3, 64)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x40, 0xB9 }, typeof(ConfigCVMessage), 1234, 4, 64)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x02, 0x48, 0xB0 }, typeof(ConfigCVMessage), 1234, 3, 72)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0xEC, 0x03, 0x48, 0xB1 }, typeof(ConfigCVMessage), 1234, 4, 72)]
    public void Test(byte[] packet, Type expectedType, int address, int cvNum, int cvValue) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.TypeOf(expectedType));
        Assert.That(decoded.AddressType, Is.AnyOf(AddressTypeEnum.Long, AddressTypeEnum.Short, AddressTypeEnum.Broadcast));
        Assert.That(decoded.Address, Is.EqualTo(address));
        Assert.That(((ConfigCVMessage)decoded).CvNumber, Is.EqualTo(cvNum));
        Assert.That(((ConfigCVMessage)decoded).CvValue, Is.EqualTo(cvValue));
    }
}