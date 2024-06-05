using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class MomentumMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<MomentumMessage> {
    public MomentumTypeEnum Type  { get; }
    public byte             Value { get; }

    public MomentumMessage(IPacketMessage packet, MomentumTypeEnum type, byte value) : this(packet) {
        Type  = type;
        Value = value;
    }

    public override string Summary => $"{AddressAsString} {Type}:{Value}";

    public override string ToString() {
        return FormatHelper.FormatMessage("MOMENTUM", base.ToString(), PacketData, ("Type", Type.ToString()), ("Value", Value));
    }

    public bool Equals(MomentumMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return Type == other.Type && Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MomentumMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, (int)Type, Value);
    }
}

public enum MomentumTypeEnum {
    Acceleration = 23,
    Deceleration = 24,
    Lock         = 0
}