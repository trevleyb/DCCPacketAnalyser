using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Base;

public abstract class PacketMessage(PacketData packetData) {
    protected PacketData      PacketData  { get; } = packetData;
    public    int             Address     { get; protected set; }
    public    AddressTypeEnum AddressType { get; protected set; }
}
