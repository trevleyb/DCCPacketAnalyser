using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class AnalogFunctionMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, AddressTypeEnum.Accessory, packet.Address) {
    public override string ToString() {
        return $"ANALOG FUNCTION: Address={Address}\t[{PacketData.ToBinary}]";
    }
}