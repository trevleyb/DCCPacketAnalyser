using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class ControlMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address) {
    public DecoderMessageTypeEnum MessageType { get; set; }
    public override string ToString() {
        return $"CONTROL: Address={Address} \t[{PacketData.ToBinary}]";
    }
}