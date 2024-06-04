using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class BinaryStateMessage(IPacketMessage packet, BinaryStateTypeEnum state) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<BinaryStateMessage> {
    public BinaryStateTypeEnum State { get; init; } = state;
    public override string ToString() {
        return FormatHelper.FormatMessage("BINARY STATE", base.ToString(), PacketData );
    }

    public bool Equals(BinaryStateMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return State == other.State;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((BinaryStateMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine((int)Address,(int)State);
    }
}

public enum BinaryStateTypeEnum {
    Long, 
    Short
}