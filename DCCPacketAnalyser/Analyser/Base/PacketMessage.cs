using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Base;

public class PacketMessage(PacketData packetData, AddressTypeEnum addressType, int address) : IPacketMessage {
    public PacketData      PacketData  { get; } = packetData;
    public int             Address     { get; protected init; } = address;
    public AddressTypeEnum AddressType { get; protected init; } = addressType;
}