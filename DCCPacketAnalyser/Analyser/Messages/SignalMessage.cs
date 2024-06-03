using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class SignalMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, AddressTypeEnum.Signal, packet.Address) {
    public SignalAspectEnums Aspect { get; set; } = SignalAspectEnums.AllOff;

    public override string ToString() {
        return $"SIGNAL: Address={Address} Aspect={Aspect} ({(int)Aspect})\t[{PacketData.ToBinary}]";
    }
}