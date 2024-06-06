using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Tests;

[TestFixture]
public class PacketAnalyserTest {
    [TestCase(new byte[] { 195, 252, 63, 130, 130 }, AddressTypeEnum.Long, 1020)]
    public void TestAddressDecoder(byte[] packet, AddressTypeEnum addressType, int address) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.InstanceOf<SpeedAndDirectionMessage>());

        decoded = decoded as PacketMessage;
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded.AddressType, Is.EqualTo(addressType));
        Assert.That(decoded.Address, Is.EqualTo(address));
    }

    [TestCase(new byte[] { 0x81, 0xF9, 0x78 }, 1, AccessoryStateEnum.Normal)]
    [TestCase(new byte[] { 0x81, 0xF8, 0x79 }, 1, AccessoryStateEnum.Reversed)]
    public void TestAccessory(byte[] packet, int address, AccessoryStateEnum state) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet) as AccessoryMessage;
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.TypeOf<AccessoryMessage>());
        Assert.That(decoded.AddressType, Is.EqualTo(AddressTypeEnum.Accessory));
        Assert.That(decoded.Address, Is.EqualTo(address));
        Console.WriteLine(decoded.ToString());
    }

    [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA }, 1, 10)]
    [TestCase(new byte[] { 0x83, 0x73, 0x06, 0xF6 }, 10, 6)]
    [TestCase(new byte[] { 0x83, 0x77, 0x01, 0xF5 }, 12, 1)]
    public void TestSignal(byte[] packet, int address, byte aspect) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.TypeOf<SignalMessage>());

        decoded = decoded as SignalMessage;
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded.AddressType, Is.EqualTo(AddressTypeEnum.Signal));
        Assert.That(decoded.Address, Is.EqualTo(address));
        Assert.That(((SignalMessage)decoded).Aspect, Is.EqualTo((SignalAspectEnums)aspect));
        Console.WriteLine(decoded.ToString());
    }

    [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA })]
    [TestCase(new byte[] { 0x83, 0x73, 0x06, 0xF6 })]
    [TestCase(new byte[] { 0x83, 0x77, 0x01, 0xF5 })]
    public void TestCheckSum(byte[] packet) {
        var pd = new PacketData(packet);
        Assert.That(pd.IsValidPacket, Is.True);
    }
}