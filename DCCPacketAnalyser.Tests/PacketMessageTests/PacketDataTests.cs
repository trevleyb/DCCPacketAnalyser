using DCCPacketAnalyser.Analyser.Base;

namespace DCCPacketAnalyser.Tests.PacketMessageTests;

[TestFixture]
public class PacketDataTests {
    [Test]
    public void Next_ReturnsCorrectByte_WhenCalledMultipleTimes() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData.Next(), Is.EqualTo(0x01));
        Assert.That(packetData.Next(), Is.EqualTo(0x02));
        Assert.That(packetData.Next(), Is.EqualTo(0x03));
    }

    [Test]
    public void Current_ReturnsCorrectByte_WhenCalledAfterNext() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02 });
        packetData.Next();
        Assert.That(packetData.Current(), Is.EqualTo(0x01));
    }

    [Test]
    public void First_ReturnsFirstByte_WhenCalled() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02 });
        Assert.That(packetData.First(), Is.EqualTo(0x01));
    }

    [Test]
    public void Peek_ReturnsCorrectByte_WhenCalledAfterNext() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        packetData.Next();
        Assert.That(packetData.Peek(), Is.EqualTo(0x02));
    }

    [Test]
    public void Skip_ReturnsCorrectByte_WhenCalledAfterNext() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        packetData.Next();
        Assert.That(packetData.Skip(), Is.EqualTo(0x03));
    }

    [Test]
    public void GetAt_ReturnsCorrectByte_WhenCalledWithValidIndex() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData.GetAt(1), Is.EqualTo(0x02));
    }

    [Test]
    public void GetAt_ThrowsException_WhenCalledWithInvalidIndex() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(() => packetData.GetAt(3), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Elements_ReturnsCorrectCount_WhenCalled() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData.Elements, Is.EqualTo(3));
    }

    [Test]
    public void IsAtLeastLength_ReturnsTrue_WhenCalledWithLengthLessThanOrEqualToPacketLength() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData.IsAtLeastLength(2), Is.True);
    }

    [Test]
    public void IsAtLeastLength_ReturnsFalse_WhenCalledWithLengthGreaterThanPacketLength() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData.IsAtLeastLength(4), Is.False);
    }

    [Test]
    public void IsValidPacket_ReturnsTrue_WhenPacketIsValid() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData.IsValidPacket, Is.True);
    }

    [Test]
    public void IsValidPacket_ReturnsFalse_WhenPacketIsInvalid() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x04 });
        Assert.That(packetData.IsValidPacket, Is.False);
    }

    [Test]
    public void ToBinary_ReturnsCorrectString_WhenCalled() {
        var packetData = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData.ToBinary, Is.EqualTo("00000001-00000010-00000011"));
    }

    [Test]
    public void Equals_ReturnsTrue_WhenComparingIdenticalPacketData() {
        var packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        var packetData2 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData1.Equals(packetData2), Is.True);
    }

    [Test]
    public void Equals_ReturnsFalse_WhenComparingDifferentPacketData() {
        var packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        var packetData2 = new PacketData(new byte[] { 0x01, 0x02, 0x04 });
        Assert.That(packetData1.Equals(packetData2), Is.False);
    }

    [Test]
    public void GetHashCode_ReturnsSameHashCode_WhenCalledOnIdenticalPacketData() {
        var packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        var packetData2 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData1.GetHashCode(), Is.EqualTo(packetData2.GetHashCode()));
    }

    [Test]
    public void GetHashCode_ReturnsDifferentHashCode_WhenCalledOnDifferentPacketData() {
        var packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        var packetData2 = new PacketData(new byte[] { 0x01, 0x02, 0x04 });
        Assert.That(packetData1.GetHashCode(), Is.Not.EqualTo(packetData2.GetHashCode()));
    }

    [Test]
    public void EqualsObject_ReturnsTrue_WhenComparingIdenticalPacketData() {
        var    packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        object packetData2 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        Assert.That(packetData1.Equals(packetData2), Is.True);
    }

    [Test]
    public void EqualsObject_ReturnsFalse_WhenComparingDifferentPacketData() {
        var    packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        object packetData2 = new PacketData(new byte[] { 0x01, 0x02, 0x04 });
        Assert.That(packetData1.Equals(packetData2), Is.False);
    }

    [Test]
    public void EqualsObject_ReturnsFalse_WhenComparingWithNull() {
        var     packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        object? packetData2 = null;
        Assert.That(packetData1.Equals(packetData2), Is.False);
    }

    [Test]
    public void EqualsObject_ReturnsFalse_WhenComparingWithDifferentType() {
        var    packetData1 = new PacketData(new byte[] { 0x01, 0x02, 0x03 });
        object packetData2 = new byte[] { 0x01, 0x02, 0x03 };
        Assert.That(packetData1.Equals(packetData2), Is.False);
    }
}