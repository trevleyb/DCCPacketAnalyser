using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class ConsistMessage(IPacketMessage packet, int address, DirectionEnum direction) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<ConsistMessage> {

    public DirectionEnum Direction      { get; init; } = direction;
    public int           ConsistAddress { get; init; } = address;

    public override string Summary => $"{AddressAsString} {ConsistAddress}{Direction switch {DirectionEnum.Forward => "F", DirectionEnum.Reverse => "R", DirectionEnum.Stop => "S", DirectionEnum.EStop => "E", _ => "?"}}"; 
    public override string ToString() {
        return FormatHelper.FormatMessage("CONSIST", base.ToString(), PacketData, ("Address",ConsistAddress),("Direction",Direction.ToString()) );
    }

    public bool Equals(ConsistMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return Direction == other.Direction && ConsistAddress == other.ConsistAddress;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ConsistMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, (int)Direction, ConsistAddress);
    }
}