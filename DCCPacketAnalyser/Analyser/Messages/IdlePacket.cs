using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class IdlePacket : PacketMessage, IPacketMessage {
    public IdlePacket(PacketData packetData) : base(packetData) {
        Address     = 0;
        AddressType = AddressTypeEnum.Idle;
    }

    public override string ToString() {
        return $"IDLE Packet";
    }

    public void ProcessRemainingPacket() { }
}