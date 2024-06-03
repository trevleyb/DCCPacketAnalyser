using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class SpeedAndDirMessage (IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address) {

    public DirectionEnum Direction       { get; set; }
    public byte          Speed           { get; set; }
    public bool          RestrictedSpeed { get; set; }

    public override string ToString() {
        return $"SPEED: Address={Address} \t[{PacketData.ToBinary}]";
    }
}