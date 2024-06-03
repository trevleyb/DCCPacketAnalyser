using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class ConsistMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address) {

    public DirectionEnum Direction      { get; set; }
    public int           ConsistAddress { get; set; }

    public override string ToString() {
        return $"CONSIST: Address={Address} \t[{PacketData.ToBinary}]";
    }
}