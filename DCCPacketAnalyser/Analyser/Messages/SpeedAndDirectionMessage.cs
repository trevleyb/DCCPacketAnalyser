using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class SpeedAndDirectionMessage(IPacketMessage packet, byte speed, DirectionEnum direction, bool isRestricted = false) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<SpeedAndDirectionMessage> {
    public DirectionEnum Direction       { get; } = direction;
    public byte          Speed           { get; } = speed;
    public bool          RestrictedSpeed { get; } = isRestricted;

    public override string Summary => $"{AddressAsString} S{Speed}{Direction switch { DirectionEnum.Forward => "F", DirectionEnum.Reverse => "R", DirectionEnum.Stop => "S", DirectionEnum.EStop => "E", _ => "?" }}";

    public override string ToString() {
        return FormatHelper.FormatMessage("SPEED & DIR", base.ToString(), PacketData, ("Speed", Speed), ("Direction", Direction.ToString()), ("IsRestricted?", RestrictedSpeed.ToString()));
    }

    public bool Equals(SpeedAndDirectionMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return Direction == other.Direction && Speed == other.Speed && RestrictedSpeed == other.RestrictedSpeed;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((SpeedAndDirectionMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, (int)Direction, Speed, RestrictedSpeed);
    }
}

public enum DirectionEnum {
    Forward,
    Reverse,
    Stop,
    EStop,
    Unknown
}