using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class AnalogFunctionMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, AddressTypeEnum.Accessory, packet.Address), IEquatable<AnalogFunctionMessage> {
    public override string Summary => $"{AddressAsString}";

    public override string ToString() {
        return FormatHelper.FormatMessage("ANALOG", base.ToString(), PacketData);
    }

    public bool Equals(AnalogFunctionMessage? other) {
        return Address == other?.Address;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((AnalogFunctionMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address);
    }
}