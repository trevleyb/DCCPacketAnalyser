using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Base;

public interface IPacketMessage {
    int             Address     { get; }
    AddressTypeEnum AddressType { get; }
    PacketData      PacketData  { get; }
}