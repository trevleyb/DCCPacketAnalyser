using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class IdleMessage : PacketMessage, IPacketMessage {
    public IdleMessage(PacketData packetData) : base(packetData, AddressTypeEnum.Idle, 0) { }

    public override string ToString() {
        return $"IDLE Packet\t[{PacketData.ToBinary}]";
    }

    public void ProcessRemainingPacket() { }
}