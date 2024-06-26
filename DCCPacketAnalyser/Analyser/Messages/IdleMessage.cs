using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class IdleMessage(PacketData packetData) : PacketMessage(packetData, AddressTypeEnum.Idle, 0), IEquatable<IdleMessage> {
    public override string Summary => "IDLE";

    public override string ToString() {
        return FormatHelper.FormatMessage("IDLE Packet", base.ToString(), PacketData);
    }

    public bool Equals(IdleMessage? other) {
        return true;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((IdleMessage)obj);
    }

    public override int GetHashCode() {
        return 0;
    }
}