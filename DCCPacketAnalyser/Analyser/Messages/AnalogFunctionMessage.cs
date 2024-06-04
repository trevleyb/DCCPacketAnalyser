using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class AnalogFunctionMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, AddressTypeEnum.Accessory, packet.Address), IEquatable<AnalogFunctionMessage> {
    public override string ToString() {
        return FormatHelper.FormatMessage("ANALOG", base.ToString(), PacketData );
    }

    public bool Equals(AnalogFunctionMessage? other) {
        if (Address != other?.Address) return false;
        return true;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AnalogFunctionMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine((int)Address);
    }
}