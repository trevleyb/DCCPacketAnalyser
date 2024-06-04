using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Tests.MessageTypeTests;

[TestFixture]
public class TestSpeedAndDirectionMessage {

    [TestCase(new byte[] { 0x43, 0x3F, 0x82, 0xFE }, typeof(SpeedAndDirectionMessage), 67, 1, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x83, 0xFF }, typeof(SpeedAndDirectionMessage), 67, 2, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x84, 0xF8 }, typeof(SpeedAndDirectionMessage), 67, 3, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x85, 0xF9 }, typeof(SpeedAndDirectionMessage), 67, 4, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x86, 0xFA }, typeof(SpeedAndDirectionMessage), 67, 5, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x87, 0xFB }, typeof(SpeedAndDirectionMessage), 67, 6, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x88, 0xF4 }, typeof(SpeedAndDirectionMessage), 67, 7, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x89, 0xF5 }, typeof(SpeedAndDirectionMessage), 67, 8, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x8A, 0xF6 }, typeof(SpeedAndDirectionMessage), 67, 9, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x8B, 0xF7 }, typeof(SpeedAndDirectionMessage), 67, 10, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x95, 0xE9 }, typeof(SpeedAndDirectionMessage), 67, 20, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0x9F, 0xE3 }, typeof(SpeedAndDirectionMessage), 67, 30, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xA9, 0xD5 }, typeof(SpeedAndDirectionMessage), 67, 40, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xB3, 0xCF }, typeof(SpeedAndDirectionMessage), 67, 50, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xBD, 0xC1 }, typeof(SpeedAndDirectionMessage), 67, 60, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xC7, 0xBB }, typeof(SpeedAndDirectionMessage), 67, 70, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xD1, 0xAD }, typeof(SpeedAndDirectionMessage), 67, 80, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xDB, 0xA7 }, typeof(SpeedAndDirectionMessage), 67, 90, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xE5, 0x99 }, typeof(SpeedAndDirectionMessage), 67, 100, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xEF, 0x93 }, typeof(SpeedAndDirectionMessage), 67, 110, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xF9, 0x85 }, typeof(SpeedAndDirectionMessage), 67, 120, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x3F, 0xFF, 0x83 }, typeof(SpeedAndDirectionMessage), 67, 126, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0x3F, 0x8B, 0xA2, }, typeof(SpeedAndDirectionMessage), 1234, 10, DirectionEnum.Forward)]
    [TestCase(new byte[] { 0x43, 0x61, 0x22 }, typeof(SpeedAndDirectionMessage), 67, 0, DirectionEnum.Stop)]
    [TestCase(new byte[] { 0xC4, 0xD2, 0x60, 0x76 }, typeof(SpeedAndDirectionMessage), 1234, 0, DirectionEnum.Stop)]
    public void Test(byte[] packet, Type expectedType, int address, int speed, DirectionEnum direction) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.TypeOf(expectedType));
        Assert.That(decoded.AddressType, Is.AnyOf(AddressTypeEnum.Long, AddressTypeEnum.Short, AddressTypeEnum.Broadcast));
        Assert.That(decoded.Address, Is.EqualTo(address));
        Assert.That(((SpeedAndDirectionMessage)decoded).Speed, Is.EqualTo(speed));
        Assert.That(((SpeedAndDirectionMessage)decoded).Direction, Is.EqualTo(direction));
    }
}