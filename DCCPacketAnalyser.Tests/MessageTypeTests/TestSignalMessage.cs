using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Tests.MessageTypeTests;

[TestFixture]
public class TestSignalMessage {
    
    [TestCase(new byte[] { 0x81, 0x71, 0x01, 0xF1 }, typeof(SignalMessage), 1, SignalAspectEnums.Yellow)]
    [TestCase(new byte[] { 0x81, 0x71, 0x02, 0xF2 }, typeof(SignalMessage), 1, SignalAspectEnums.Green)]
    [TestCase(new byte[] { 0x81, 0x71, 0x03, 0xF3 }, typeof(SignalMessage), 1, SignalAspectEnums.FlashRed)]
    [TestCase(new byte[] { 0x81, 0x71, 0x04, 0xF4 }, typeof(SignalMessage), 1, SignalAspectEnums.FlashYellow)]
    [TestCase(new byte[] { 0x81, 0x71, 0x05, 0xF5 }, typeof(SignalMessage), 1, SignalAspectEnums.FlashGreen)]
    [TestCase(new byte[] { 0x81, 0x71, 0x06, 0xF6 }, typeof(SignalMessage), 1, SignalAspectEnums.RedYellow)]
    [TestCase(new byte[] { 0x81, 0x71, 0x07, 0xF7 }, typeof(SignalMessage), 1, SignalAspectEnums.FlashRedYellow)]
    [TestCase(new byte[] { 0x81, 0x71, 0x08, 0xF8 }, typeof(SignalMessage), 1, SignalAspectEnums.RedFlashYellow)]
    [TestCase(new byte[] { 0x81, 0x71, 0x09, 0xF9 }, typeof(SignalMessage), 1, SignalAspectEnums.RedGreen)]
    [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA }, typeof(SignalMessage), 1, SignalAspectEnums.FlashRedGreen)]
    [TestCase(new byte[] { 0x81, 0x71, 0x0B, 0xFB }, typeof(SignalMessage), 1, SignalAspectEnums.RedFlashGreen)]
    [TestCase(new byte[] { 0x81, 0x71, 0x0C, 0xFC }, typeof(SignalMessage), 1, SignalAspectEnums.YellowGreen)]
    [TestCase(new byte[] { 0x81, 0x71, 0x0D, 0xFD }, typeof(SignalMessage), 1, SignalAspectEnums.FlashYellowGreen)]
    [TestCase(new byte[] { 0x81, 0x71, 0x0E, 0xFE }, typeof(SignalMessage), 1, SignalAspectEnums.YellowFlashGreen)]
    [TestCase(new byte[] { 0x81, 0x71, 0x0F, 0xFF }, typeof(SignalMessage), 1, SignalAspectEnums.AllOn)]
    [TestCase(new byte[] { 0x81, 0x71, 0x10, 0xE0 }, typeof(SignalMessage), 1, 16)]
    [TestCase(new byte[] { 0x81, 0x71, 0x11, 0xE1 }, typeof(SignalMessage), 1, 17)]
    [TestCase(new byte[] { 0x81, 0x71, 0x12, 0xE2 }, typeof(SignalMessage), 1, 18)]
    [TestCase(new byte[] { 0x81, 0x71, 0x13, 0xE3 }, typeof(SignalMessage), 1, 19)]
    [TestCase(new byte[] { 0x81, 0x71, 0x14, 0xE4 }, typeof(SignalMessage), 1, 20)]
    [TestCase(new byte[] { 0x81, 0x71, 0x1E, 0xEE }, typeof(SignalMessage), 1, SignalAspectEnums.AllFlash)]
    [TestCase(new byte[] { 0x81, 0x71, 0x1F, 0xEF }, typeof(SignalMessage), 1, SignalAspectEnums.AllOff)]
    public void Test(byte[] packet, Type expectedType, int address, SignalAspectEnums aspect) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.TypeOf(expectedType));
        Assert.That(decoded.AddressType, Is.EqualTo(AddressTypeEnum.Signal));
        Assert.That(decoded.Address, Is.EqualTo(address));
        Assert.That(((SignalMessage)decoded).Aspect, Is.EqualTo(aspect));
    }
}