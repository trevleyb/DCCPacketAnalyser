using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class AccessoryMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, AddressTypeEnum.Accessory, packet.Address) {
    public AccessoryStateEnum State { get; set; }

    public override string ToString() {
        return $"ACCESSORY: Address={Address} State={State}\t[{PacketData.ToBinary}]";
    }
}