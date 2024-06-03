using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class BinaryStateMessage(IPacketMessage packet, BinaryStateTypeEnum state) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address) {
    public BinaryStateTypeEnum State { get; set; } = state;
    public override string ToString() {
        return $"BINARY STATE: Address={Address} \t[{PacketData.ToBinary}]";
    }
}

public enum BinaryStateTypeEnum {
    Long, 
    Short
}