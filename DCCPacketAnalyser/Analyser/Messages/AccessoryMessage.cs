using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class AccessoryMessage(IPacketMessage packet, AccessoryStateEnum state) : PacketMessage(packet.PacketData, AddressTypeEnum.Accessory, packet.Address), IEquatable<AccessoryMessage> {
    public AccessoryStateEnum State { get; init; } = state;

    public override string Summary => $"{AddressAsString}{State switch {AccessoryStateEnum.On => "N", AccessoryStateEnum.Off => "R", _ => "?"}}"; 
    public override string ToString() {
        return FormatHelper.FormatMessage("ACCESSORY", base.ToString(), PacketData, ("State",state.ToString()) );
    }

    public bool Equals(AccessoryMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other?.Address) return false;
        return State == other.State;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AccessoryMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, (int)State);
    }
}

public enum AccessoryStateEnum {
    Normal   = 0,
    Reversed = 1,
    Off      = 0,
    On       = 1
}
