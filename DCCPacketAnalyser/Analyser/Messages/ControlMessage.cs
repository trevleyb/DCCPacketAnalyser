using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class ControlMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<ControlMessage> {
    public ControlMessageTypeEnum MessageType { get; init; }

    public override string Summary     => $"{AddressAsString} {MessageType}"; 
    public override string ToString() {
        return FormatHelper.FormatMessage("CONTROL MSG", base.ToString(), PacketData, ("Control Type",MessageType.ToString()) );
    }

    public bool Equals(ControlMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return MessageType == other.MessageType;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ControlMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine((int)Address, (int)MessageType);
    }
}

public enum ControlMessageTypeEnum {
    None,
    Reserved,
    DecoderAck,
    DecoderFlags,
    AdvancedAddressing,
    Reset,
    HardReset,
    FactoryTest,
}
