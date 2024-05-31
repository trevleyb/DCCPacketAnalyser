using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class BroadcastPacket : PacketMessage, IPacketMessage {
    public BroadcastPacket(PacketData packetData) : base (packetData) {
        Address     = 0;
        AddressType = AddressTypeEnum.Broadcast;
    }

    public override string ToString() => $"BROADCAST: Address={Address}";
    public          void   ProcessRemainingPacket() { }

}