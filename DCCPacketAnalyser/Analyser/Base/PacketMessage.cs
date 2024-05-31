using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Base;

public abstract class PacketMessage(PacketData packetData) {
    public PacketData      PacketData  { get; set; } = packetData;
    public int             Address     { get; set; }
    public AddressTypeEnum AddressType { get; set; }
}
