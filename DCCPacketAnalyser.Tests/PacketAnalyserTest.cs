using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Tests;

[TestFixture]
public class PacketAnalyserTest {
    private PacketAnalyser _packetAnalyser;

    [SetUp]
    public void SetUp() {
        _packetAnalyser = new PacketAnalyser();
    }

    [Test]
    public void DefaultConstructor_CreatesInstance() {
        var packetAnalyser = new PacketAnalyser();
        Assert.That(packetAnalyser, Is.Not.Null);
    }

    [TestCase(new byte[] { 195, 252, 63, 130, 130 }, AddressTypeEnum.Long, 1020)]
    public void TestAddressDecoderUsingBytes(byte[] packet, AddressTypeEnum addressType, int address) {
        var decoder = new PacketAnalyser();
        var decoded = decoder.Decode(packet);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.InstanceOf<SpeedAndDirectionMessage>());

        decoded = decoded as PacketMessage;
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded.AddressType, Is.EqualTo(addressType));
        Assert.That(decoded.Address, Is.EqualTo(address));
    }

    [TestCase(new byte[] { 195, 252, 63, 130, 130 }, AddressTypeEnum.Long, 1020)]
    public void TestAddressDecoderUsingString(byte[] packet, AddressTypeEnum addressType, int address) {
        var packetAsString = BitConverter.ToString(packet).Replace("-", " ");
        var decoder        = new PacketAnalyser();
        var decoded        = decoder.Decode(packetAsString);
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded, Is.InstanceOf<SpeedAndDirectionMessage>());

        decoded = decoded as PacketMessage;
        Assert.That(decoded, Is.Not.Null);
        Assert.That(decoded.AddressType, Is.EqualTo(addressType));
        Assert.That(decoded.Address, Is.EqualTo(address));
    }

    [TestCase(new byte[] { 195, 252, 63, 130, 130 }, AddressTypeEnum.Long, 1020)]
    public void TestAddressDecoderUsingPacketData(byte[] packet, AddressTypeEnum addressType, int address) {
        var decoder    = new PacketAnalyser();
        var packetData = new PacketData(packet);
        var decoded    = decoder.Decode(packetData);
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

    [Test]
    public void Decode_ReturnsErrorMessage_WhenGivenConsistentlyInvalidPacketData() {
        var packet = new byte[] { 0x01, 0x02, 0x04 };
        for (var i = 0; i < 10; i++) {
            var result = _packetAnalyser.Decode(packet);
            if (i < 10) {
                Assert.That(result, Is.TypeOf<ErrorMessage>());
            } else {
                Assert.Throws<Exception>(() => _packetAnalyser.Decode(packet));
            }
        }
    }

    [Test]
    public void SpeedAndDirection_ReturnsCorrectMessage_WhenGivenValidData() {
        var packetMessage = new PacketMessage(new PacketData(new byte[] { 0x01, 0x02, 0x03 }));
        var result        = _packetAnalyser.SpeedAndDirection(packetMessage, 0x01, DirectionEnum.Forward);
        Assert.That(result, Is.TypeOf<SpeedAndDirectionMessage>());
    }

    [Test]
    public void FunctionGroupOneInstructions_ReturnsCorrectMessage_WhenGivenValidData() {
        var packetMessage = new PacketMessage(new PacketData(new byte[] { 0x01, 0x02, 0x03 }));
        var result        = _packetAnalyser.FunctionGroupOneInstructions(packetMessage, 0x01);
        Assert.That(result, Is.TypeOf<FunctionsMessage>());
    }

    [Test]
    public void FunctionGroupTwoInstructions_ReturnsCorrectMessage_WhenGivenValidData() {
        var packetMessage = new PacketMessage(new PacketData(new byte[] { 0x01, 0x02, 0x03 }));
        var result        = _packetAnalyser.FunctionGroupTwoInstructions(packetMessage, 0x01);
        Assert.That(result, Is.TypeOf<FunctionsMessage>());
    }
}