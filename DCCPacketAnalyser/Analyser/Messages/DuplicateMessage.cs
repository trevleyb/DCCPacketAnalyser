using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class DuplicateMessage(PacketData packetData) : PacketMessage(packetData, AddressTypeEnum.Duplicate, 0), IPacketMessage {
    public override string ToString() {
        return $"DUPLICATE Packet\t[{PacketData.ToBinary}]";
    }

    public void ProcessRemainingPacket() { }
}